
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static NetInterop.Transport.Core.Packets.PacketControllerExtensions;
using NetInterop.Transport.Core.Abstractions.Packets;
using NetInterop.Transport.Core.Packets;

namespace NetInterop.Transport.Core.Packets
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
