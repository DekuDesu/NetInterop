using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetInterop.Transport.Core.Packets;

namespace NetInterop.Transport.Core.Abstractions.Packets
{
    /// <summary>
    /// Defines an object that implements a method that should sort and dispatch a packet based on it's properties to they can be eventually be consumed.
    /// </summary>
    /// <typeparam name="TPacketType"></typeparam>
    public interface IPacketDispatcher<TPacketType> where TPacketType : Enum
    {
        /// <summary>
        /// Dispatches the provided packet to the appropriate handler to eventually be consumed.
        /// </summary>
        /// <param name="packet"></param>
        void Dispatch(IPacket<TPacketType> packet);
    }
}
