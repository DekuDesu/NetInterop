using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading;

namespace RemoteInvoke.Net.Transport
{
    /// <summary>
    /// Reponsible for waiting for responses from the stream. When a response is received it copies the data into a new stream and dispatches it.
    /// </summary>
    public interface IPacketDispatcher
    {
        bool Dispatching { get; }
        int PollingRate { get; set; }

        void BeginDispatchingPackets(Action<Stream, int> packetHandler, CancellationToken token);
        void Cancel(CancellationToken token);
        bool TryGetPacket([NotNullWhen(true)] out Stream stream, out int packetType);
        T? WaitAndConvertPacket<T>(Func<(Stream Packet, int PacketType), T> packetConverter);
        T? WaitAndConvertPacket<T>(Func<(Stream Packet, int PacketType), T> packetConverter, CancellationToken token);
        (Stream Packet, int PacketType) WaitForPacket();
        (Stream Packet, int PacketType) WaitForPacket(CancellationToken token);
    }
}