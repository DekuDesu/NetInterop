using RemoteInvoke.Net.Transport.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static RemoteInvoke.Net.Transport.Packets.PacketControllerExtensions;

namespace RemoteInvoke.Net.Transport.Packets
{
    /// <summary>
    /// Does nothing.
    /// </summary>
    /// <typeparam name="TPacketType"></typeparam>
    public class StubPacketHandler<TPacketType> : IPacketHandler<TPacketType> where TPacketType : Enum, IConvertible
    {
        /// <summary>
        /// Does nothing.
        /// </summary>
        /// <typeparam name="TPacketType"></typeparam>
        public StubPacketHandler(TPacketType packetType)
        {
            PacketType = packetType;
        }

        /// <summary>
        /// Does nothing.
        /// </summary>
        /// <typeparam name="TPacketType"></typeparam>
        public StubPacketHandler(TPacketType packetType, PacketRefFunc<string, TPacketType> converter)
        {
            _ = converter;
            PacketType = packetType;
        }

        public TPacketType PacketType { get; set; }

        /// <summary>
        /// Does nothing.
        /// </summary>
        /// <param name="packet"></param>
        public void Handle(ref Packet<TPacketType> packet)
        {
            return;
        }
    }
}
