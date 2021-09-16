using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetInterop.Transport.Core.Packets;
using NetInterop.Transport.Core.Abstractions;
using NetInterop.Transport.Core.Abstractions.Packets;

using static NetInterop.Transport.Core.Packets.PacketControllerExtensions;

namespace RNetInterop.Transport.Core.Packets
{
    public class ActionPacketHandler<TPacketType> : IPacketHandler<TPacketType> where TPacketType : Enum, IConvertible
    {
        private readonly PacketRefAction<TPacketType> action;

        public ActionPacketHandler(TPacketType packetType, PacketRefAction<TPacketType> action)
        {
            this.PacketType = packetType;
            this.action = action;
        }

        public TPacketType PacketType { get; set; }

        public void Handle(ref Packet<TPacketType> packet)
        {
            action(ref packet);
        }
    }
}
