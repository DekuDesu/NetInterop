using NetInterop.Transport.Core.Abstractions.Packets;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetInterop
{
    public class DefaultPointerResponseSender : IPointerResponseSender
    {
        private readonly IPacketSender sender;

        public DefaultPointerResponseSender(IPacketSender sender)
        {
            this.sender = sender;
        }

        public void SendBadResponse(ushort callbackId)
        {
            sender.Send(new PointerOperationPacket(PointerOperations.OperationResult, new PointerResponsePacket(false, new CallbackResponsePacket(callbackId))));
        }

        public void SendGoodResponse(ushort callbackId)
        {
            sender.Send(new PointerOperationPacket(PointerOperations.OperationResult, new PointerResponsePacket(true, new CallbackResponsePacket(callbackId))));
        }

        public void SendResponse(ushort callbackId, bool goodResponse, IPacketSerializable packetBuilder)
        {
            sender.Send(new PointerOperationPacket(PointerOperations.OperationResult, new PointerResponsePacket(goodResponse, new CallbackResponsePacket(callbackId, packetBuilder))));
        }
    }
}
