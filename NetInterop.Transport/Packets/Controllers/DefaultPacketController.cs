using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NetInterop.Transport.Core.Packets;
using NetInterop.Transport.Core.Abstractions;
using NetInterop.Transport.Core.Abstractions.Packets;

#nullable enable
namespace NetInterop.Transport.Core.Packets
{
    public class DefaultPacketController<T> : IPacketController<T> where T : Enum, IConvertible
    {
        private readonly IStream<byte> backingStream;
        private readonly IPacketHeader<T> headerParser;
        private const int PollingRate = 1;
        private readonly SemaphoreSlim locker = new(1, 1);
        public bool PendingPackets => backingStream.DataAvailable;

        public DefaultPacketController(IStream<byte> backingStream, IPacketHeader<T> headerParser)
        {
            this.backingStream = backingStream ?? throw new ArgumentNullException(nameof(backingStream));
            this.headerParser = headerParser ?? throw new ArgumentNullException(nameof(headerParser));
        }

        public IPacket<T> WaitForPacket(CancellationToken token = default)
        {
            while (token.IsCancellationRequested is false)
            {
                if (TryReadPacket(out var packet))
                {
                    return packet;
                }
                Thread.Sleep(PollingRate);
            }

            return default;
        }

        public bool TryReadPacket(out IPacket<T> packet)
        {
            packet = default;
            if (backingStream.DataAvailable is false)
            {
                return false;
            }

            locker.Wait();
            try
            {
                if (backingStream.DataAvailable)
                {
                    byte[] header = new byte[4];

                    // read the header, it contains the type of packet and the size of the packet
                    backingStream.Read(header);

                    ref byte headerPtr = ref header[0];

                    // convert the int type to the actual packet type for compile type type safety and convenience
                    T packetType = headerParser.GetHeaderType(ref headerPtr);

                    // get the message size in bytes
                    int messageSize = headerParser.GetPacketSize(ref headerPtr);

                    // this is for packets that hold no data and merely represent messages
                    // such as things like "response good" or "ping"
                    if (messageSize == 0)
                    {
                        packet = Packet.Empty(packetType);

                        return true;
                    }

                    byte[] packetData = new byte[messageSize];

                    int bytesRead = backingStream.Read(packetData);

                    bool validPacket = bytesRead == messageSize;

                    if (validPacket)
                    {
                        // no sense creating a new object if we got weird data
                        packet = Packet.Create(packetType, packetData);

                        // this is purely for consistency and debugging, header bytes aren't used for reading gener
                        packet.SetHeader(header);
                    }

                    return validPacket;
                }
            }
            finally
            {
                locker.Release();
            }

            // if there was no data available then return false
            return false;
        }

        public bool TryWritePacket(IPacket<T> packet, out IPacket<T> responsePacket, CancellationToken token = default)
        {
            WriteBlindPacket(packet);

            responsePacket = WaitForPacket(token);

            return responsePacket.PacketType.ToInt32(null) != 0;
        }

        public void WriteBlindPacket(IPacket<T> packet)
        {
            headerParser.CreateHeader(packet);

            backingStream.Write(packet.GetData());
        }
    }
}
