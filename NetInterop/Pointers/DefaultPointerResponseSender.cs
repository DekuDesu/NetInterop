using NetInterop.Transport.Core.Abstractions.Packets;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetInterop
{
    public class DefaultPointerResponseSender<TPacket> : IPointerResponseSender where TPacket : Enum, IConvertible
    {
        private readonly IPacketSender<TPacket> sender;

        public DefaultPointerResponseSender(IPacketSender<TPacket> sender)
        {
            this.sender = sender;
        }

        public void SendBadResponse()
        {
            sender.Send(new PointerResponsePacket<TPacket>(false));
        }

        public void SendGoodResponse()
        {
            sender.Send(new PointerResponsePacket<TPacket>(true));
        }

        public void SendResponse(bool goodResponse, IPacketSerializable packetBuilder)
        {
            sender.Send(new PointerResponsePacket<TPacket>(goodResponse, packetBuilder));
        }
    }
}
