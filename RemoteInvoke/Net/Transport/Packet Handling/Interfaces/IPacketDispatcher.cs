using System;
using System.Threading;

namespace RemoteInvoke.Net.Transport.Packets
{
    public interface IPacketDispatcher<T> where T : Enum, IConvertible
    {
        bool TryReadPacket(out Packet<T> packet);
        Packet<T> WaitForPacket(CancellationToken token = default);
    }
}