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

        public void SendBadResponse(ushort callbackId)
        {
            sender.Send(new PointerOperationPacket<TPacket>(PointerOperations.OperationResult, new PointerResponsePacket<TPacket>(false, new CallbackResponsePacket(callbackId))));
        }

        public void SendGoodResponse(ushort callbackId)
        {
            sender.Send(new PointerOperationPacket<TPacket>(PointerOperations.OperationResult, new PointerResponsePacket<TPacket>(true, new CallbackResponsePacket(callbackId))));
        }

        public void SendResponse(ushort callbackId, bool goodResponse, IPacketSerializable packetBuilder)
        {
            sender.Send(new PointerOperationPacket<TPacket>(PointerOperations.OperationResult, new PointerResponsePacket<TPacket>(goodResponse, new CallbackResponsePacket(callbackId, packetBuilder))));
        }
    }
}
