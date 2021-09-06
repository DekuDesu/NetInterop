using System;
using System.Threading;

namespace RemoteInvoke.Net.Transport.Packets
{
    public interface IPacketDispatcher<T> where T : Enum, IConvertible
    {
        bool TryReadPacket(out Packet<T> packet);
        bool TryWritePacket(Packet<T> packet, out Packet<T> responsePacket, CancellationToken token = default);
        Packet<T> WaitForPacket(CancellationToken token = default);
        void WriteBlindPacket(Packet<T> packet);
    }
}