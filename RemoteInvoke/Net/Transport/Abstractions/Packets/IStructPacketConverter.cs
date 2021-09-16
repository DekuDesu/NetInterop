using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemoteInvoke.Net.Transport.Abstractions
{
    /// <summary>
    /// Defines an object that implements a method to convert any given unmanaged struct into a packet
    /// </summary>
    /// <typeparam name="TPacketType"></typeparam>
    public interface IStructPacketConverter<TPacketType> where TPacketType : Enum
    {
        /// <summary>
        /// Converts the provided <typeparamref name="T"/> to a <see cref="Packet{TPacketType}"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns>a by reference packet, by reference is enforced for performance reasons</returns>
        Packet<TPacketType> Convert<T>(T value) where T : unmanaged;
    }
}
