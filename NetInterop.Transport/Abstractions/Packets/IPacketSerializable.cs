using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetInterop.Transport.Core.Packets;

namespace NetInterop.Transport.Core.Abstractions.Packets
{
    /// <summary>
    /// Defines an object implements functionality to serialize itself into <typeparamref name="TResult"/>
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    public interface IPacketSerializable<TPacket> where TPacket : Enum, IConvertible
    {
        /// <summary>
        /// The type or context of this object that should be appended to the header of the packet
        /// </summary>
        TPacket PacketType { get; }

        /// <summary>
        /// Returns the estimated size of the packet when this object is serialized
        /// </summary>
        /// <returns></returns>
        int EstimatePacketSize();

        /// <summary>
        /// Serializes this object to a packet to be sent with a IPacketSender
        /// </summary>
        /// <returns></returns>
        void Serialize(ref Packet<TPacket> packetBuilder);
    }
}
