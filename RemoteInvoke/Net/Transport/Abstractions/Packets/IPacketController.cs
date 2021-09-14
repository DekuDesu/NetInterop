using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RemoteInvoke.Net.Transport.Abstractions
{
    /// <summary>
    /// Defines an object that sends and recieves packets to or from a remote or local client or source
    /// </summary>
    public interface IPacketController<TPacketType> where TPacketType : Enum, IConvertible
    {
        bool TryReadPacket(out Packet<TPacketType> packet);
        bool TryWritePacket(Packet<TPacketType> packet, out Packet<TPacketType> responsePacket, CancellationToken token = default);
        Packet<TPacketType> WaitForPacket(CancellationToken token = default);
        void WriteBlindPacket(Packet<TPacketType> packet);
    }
}
