using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetInterop.Transport.Core.Packets;
namespace NetInterop.Transport.Core.Abstractions.Packets
{
    /// <summary>
    /// Defines an object that implements functionaility to consume and handle a packet's contents
    /// </summary>
    /// <typeparam name="TPacketType"></typeparam>
    public interface IPacketHandler<TPacketType> : IPacketHandler
    {
        /// <summary>
        /// The packet type this handler should ideally handle
        /// </summary>
        TPacketType PacketType { get; }
    }
    public interface IPacketHandler
    {
        /// <summary>
        /// Consumes the provided packet that was dispatched from the packet dispatcher
        /// </summary>
        /// <param name="packet"></param>
        void Handle(IPacket packet);
    }
}
