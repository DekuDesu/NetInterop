using RemoteInvoke.Net.Transport;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemoteInvoke.Net.Abstractions
{
    /// <summary>
    /// Represents an object that implements functionality to serialize into a packet to be transported over <see cref="RemoteInvoke"/>
    /// </summary>
    public interface IPacketSerializable<TPacket> where TPacket : Enum, IConvertible
    {
        /// <summary>
        /// Serializes the object into a packet
        /// </summary>
        void Serialize(ref Packet<TPacket> builder);
    }
}
