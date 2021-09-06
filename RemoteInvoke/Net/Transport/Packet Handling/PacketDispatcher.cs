using RemoteInvoke.Net.Transport.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

#nullable enable
namespace RemoteInvoke.Net.Transport.Packets
{
    public class PacketDispatcher<T> : IPacketDispatcher<T> where T : Enum, IConvertible
    {
        private readonly IStream<byte> backingStream;
        private readonly IHeaderParser headerParser;
        private readonly Type PacketEnumType = typeof(T);
        private const int PollingRate = 1;

        public PacketDispatcher(IStream<byte> backingStream, IHeaderParser headerParser)
        {
            this.backingStream = backingStream;
            this.headerParser = headerParser;
        }

        /// <summary>
        /// Waits for packet to be received
        /// <para>
        /// Blocking; Graceful Cancel;
        /// </para>
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Checks to see if a data is available, if it is creates and returns new packet.
        /// <para>
        /// Blocking; Blocks while reading data, if available, otherwise Non-Blocking;
        /// </para>
        /// </summary>
        /// <param name="packet"></param>
        /// <returns></returns>
        public bool TryReadPacket(out Packet<T> packet)
        {
            packet = default;

            if (backingStream.DataAvailable)
            {
                // read the header, it contains the type of packet and the size of the packet
                uint header = backingStream.ReadUInt();

                // convert the int type to the actual packet type for compile type type safety and convenience
                T packetType = (T)Enum.ToObject(PacketEnumType, headerParser.GetPacketType(header));

                // get the message size in bytes
                int messageSize = headerParser.GetPacketSize(header);

                // this is for packets that hold no data and merely represent messages
                // such as things like "response good" or "ping"
                if (messageSize == 0)
                {
                    packet = new Packet<T>(packetType, Array.Empty<byte>());

                    return true;
                }

                Span<byte> packetData = new byte[messageSize];

                int bytesRead = backingStream.Read(packetData);

                bool validPacket = bytesRead == messageSize;

                if (validPacket)
                {
                    // no sense creating a new object if we got weird data
                    packet = new Packet<T>(packetType, packetData);
                }

                return validPacket;
            }

            // if there was no data available then return false
            return false;
        }
    }
}
