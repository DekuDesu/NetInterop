using System;
using System.Threading;
using NetInterop.Transport.Core.Abstractions;
using NetInterop.Transport.Core.Factories;
using NetInterop.Transport.Core.Abstractions.Packets;
using NetInterop.Transport.Core.Packets.Extensions;

namespace NetInterop.Transport.Core.Packets
{
    public class DefaultPacketController : IPacketController
    {
        private readonly IStream<byte> backingStream;
        private readonly SemaphoreSlim locker = new SemaphoreSlim(1, 1);
        private readonly byte[] headerBuffer = new byte[headerSize];
        private const int headerSize = DefaultPacket.DefaultHeaderSize;

        public bool PendingPackets => backingStream.DataAvailable;

        public DefaultPacketController(IStream<byte> backingStream)
        {
            this.backingStream = backingStream ?? throw new ArgumentNullException(nameof(backingStream));
        }

        public bool TryReadPacket(out IPacket packet)
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
                    // read the header, it contains the type of packet and the size of the packet
                    backingStream.Read(headerBuffer, 0, headerSize);

                    ref byte headerPtr = ref headerBuffer[0];

                    ushort messageSize = headerPtr.ToUShort();

                    // this is for packets that hold no data and merely represent messages
                    // such as things like "response good" or "ping"
                    if (messageSize == 0)
                    {
                        packet = Packet.Empty;

                        return true;
                    }

                    packet = Packet.Create(messageSize);

                    int bytesRead = backingStream.Read(packet.GetData(), headerSize, messageSize);

                    return bytesRead == messageSize;
                }
            }
            finally
            {
                locker.Release();
            }

            // if there was no data available then return false
            return false;
        }

        public void WritePacket(IPacket packet)
        {
            packet.CompileHeader();

            backingStream.Write(packet.GetData());
        }
    }
}
