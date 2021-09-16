using RemoteInvoke.Net.Transport.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static RemoteInvoke.Net.Transport.Packets.PacketControllerExtensions;

namespace RemoteInvoke.Net.Transport.Packets
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
