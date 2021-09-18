using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NetInterop.Transport.Core.Packets;

namespace NetInterop.Transport.Core.Abstractions.Packets
{
    /// <summary>
    /// Defines an object that sends and recieves packets to or from a remote or local client or source
    /// </summary>
    public interface IPacketController<TPacketType> where TPacketType : Enum, IConvertible
    {
        /// <summary>
        /// Whether or not there is a packet waiting to be consumed from the underlying stream
        /// </summary>
        bool PendingPackets { get; }
        bool TryReadPacket(out IPacket<TPacketType> packet);
        void WriteBlindPacket(IPacket<TPacketType> packet);
    }
}
