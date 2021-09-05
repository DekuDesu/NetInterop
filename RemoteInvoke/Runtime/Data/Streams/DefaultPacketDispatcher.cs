using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using RemoteInvoke.Runtime.Data.Helpers;
#nullable enable
namespace RemoteInvoke.Runtime.Data
{
    public class DefaultPacketDispatcher : IPacketDispatcher
    {
        private readonly IStream<byte> backingStream;
        private readonly IHeaderParser headerParser;

        private volatile bool breakSentinel;

        public bool Dispatching { get; private set; }

        public int PollingRate { get; set; } = 10;

        public DefaultPacketDispatcher(IStream<byte> stream, IHeaderParser headerParser)
        {
            this.backingStream = stream;
            this.headerParser = headerParser;
        }

        public T? WaitAndConvertPacket<T>(Func<(Stream Packet, int PacketType), T> packetConverter)
        {
            return WaitAndConvertPacket<T>(packetConverter, CancellationToken.None);
        }

        public T? WaitAndConvertPacket<T>(Func<(Stream Packet, int PacketType), T> packetConverter, CancellationToken token)
        {
            (Stream? Packet, int PacketType) dispatched = WaitForPacket(token);

            if (dispatched.Packet is not null)
            {
                try
                {
                    return packetConverter(dispatched!);
                }
                finally
                {
                    dispatched.Packet.Dispose();
                }
            }

            return default;
        }

        public (Stream? Packet, int PacketType) WaitForPacket()
        {
            return WaitForPacket(CancellationToken.None);
        }

        public bool TryGetPacket([NotNullWhen(returnValue: true)] out Stream stream, out int packetType)
        {
            // check to see if there is anything to dispatch
            if (backingStream.DataAvailable)
            {
                // get the header
                uint header = backingStream.ReadUInt();

                // determine how many bytes to read from stream to dispatch
                int packetSize = headerParser.GetPacketSize(header);

                // get what kind of packet this represents, this could be "get,put,post" etc..
                packetType = headerParser.GetPacketType(header);

                // copy the bytes to a new stream 
                stream = CopyPacketToNewStream(packetSize);

                return stream != null;
            }

            packetType = 0;
            stream = default!;
            return false;
        }

        public (Stream? Packet, int PacketType) WaitForPacket(CancellationToken token)
        {
            while (true)
            {
                if (breakSentinel)
                {
                    break;
                }

                if (token.IsCancellationRequested)
                {
                    break;
                }

                if (TryGetPacket(out var stream, out int type))
                {
                    return (stream, type);
                }

                Thread.Sleep(PollingRate);
            }

            return (null, 0);
        }

        public void BeginDispatchingPackets(Action<Stream, int> packetHandler, CancellationToken token)
        {
            if (Dispatching) return;

            Dispatching = true;

            while (true)
            {
                if (breakSentinel)
                {
                    break;
                }

                if (token.IsCancellationRequested)
                {
                    break;
                }

                if (TryGetPacket(out Stream stream, out int packetType))
                {
                    // send the stream to the handler
                    packetHandler(stream, packetType);

                    // don't sleep if we just finished, there is likely to be more payloads after this
                    continue;
                }

                Thread.Sleep(PollingRate);
            }

            Dispatching = false;
        }

        public void Cancel(CancellationToken token)
        {
            breakSentinel = true;

            while (Dispatching)
            {
                token.ThrowIfCancellationRequested();
                Thread.Sleep(10);
            }

            breakSentinel = false;
        }

        private Stream CopyPacketToNewStream(int packetSize)
        {
            MemoryStream newStream = new(packetSize);

            backingStream.CopyTo(newStream, packetSize);

            newStream.Position = 0;

            return newStream;
        }
    }
}
