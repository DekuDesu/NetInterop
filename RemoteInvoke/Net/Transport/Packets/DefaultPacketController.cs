using RemoteInvoke.Net.Transport.Abstractions;
using RemoteInvoke.Net.Transport.Extensions;
using RemoteInvoke.Net.Transport.Packets.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

#nullable enable
namespace RemoteInvoke.Net.Transport.Packets
{
    public class DefaultPacketController<T> : IPacketController<T> where T : Enum, IConvertible
    {
        private readonly IStream<byte> backingStream;
        private readonly IHeaderParser headerParser;
        private readonly Type PacketEnumType = typeof(T);
        private const int PollingRate = 1;

        public DefaultPacketController(IStream<byte> backingStream, IHeaderParser headerParser)
        {
            this.backingStream = backingStream ?? throw new ArgumentNullException(nameof(backingStream));
            this.headerParser = headerParser ?? throw new ArgumentNullException(nameof(headerParser));
        }

        public Packet<T> WaitForPacket(CancellationToken token = default)
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

        public bool TryReadPacket(out Packet<T> packet)
        {
            packet = default;

            if (backingStream.DataAvailable)
            {
                Span<byte> header = new byte[4];

                // read the header, it contains the type of packet and the size of the packet
                backingStream.Read(header);

                // convert the int type to the actual packet type for compile type type safety and convenience
                T packetType = headerParser.GetHeaderType<T>(header);

                // get the message size in bytes
                int messageSize = headerParser.GetPacketSize(header);

                // this is for packets that hold no data and merely represent messages
                // such as things like "response good" or "ping"
                if (messageSize == 0)
                {
                    packet = Packet.Empty(packetType);

                    return true;
                }

                Span<byte> packetData = new byte[messageSize];

                int bytesRead = backingStream.Read(packetData);

                bool validPacket = bytesRead == messageSize;

                if (validPacket)
                {
                    // no sense creating a new object if we got weird data
                    packet = Packet.Create(packetType, packetData);

                    // this is purely for consistency and debugging, header bytes aren't used for reading gener
                    packet.SetHeaderBytes(header);
                }

                return validPacket;
            }

            // if there was no data available then return false
            return false;
        }

        public bool TryWritePacket(Packet<T> packet, out Packet<T> responsePacket, CancellationToken token = default)
        {
            WriteBlindPacket(packet);

            responsePacket = WaitForPacket(token);

            return responsePacket.PacketType.ToInt32(null) != 0;
        }

        public void WriteBlindPacket(Packet<T> packet)
        {
            headerParser.CreateHeader(ref packet);

            backingStream.Write(packet.Data);
        }
    }
}
