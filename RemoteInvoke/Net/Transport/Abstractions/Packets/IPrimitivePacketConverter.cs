using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemoteInvoke.Net.Transport.Abstractions
{
    /// <summary>
    /// Defines an object that implements methods to convert primitive types to packets
    /// </summary>
    public interface IPrimitivePacketConverter<TPacketType> where TPacketType : Enum
    {
        /// <summary>
        /// Converts the provided primitive type to a packet
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns>a by reference packet, by reference is enforced for performance purposes</returns>
        Packet<TPacketType> Convert<T>(T value) where T : unmanaged;
    }
}
