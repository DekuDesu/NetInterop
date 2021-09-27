using NetInterop.Transport.Core.Abstractions.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetInterop.Tests.CallbackTests.Stubs
{
    // this is different from the default one becuase this one lacks the PointerOperationPacket wrapping so the packet can be sorted by the server
    public class ResponseSenderStub : IPointerResponseSender
    {
        private readonly IPacketSender sender;

        public ResponseSenderStub(IPacketSender sender)
        {
            this.sender = sender;
        }

        public void SendBadResponse(ushort callbackId)
        {
            sender.Send(new PointerResponsePacket(false, new CallbackResponsePacket(callbackId)));
        }

        public void SendGoodResponse(ushort callbackId)
        {
            sender.Send(new PointerResponsePacket(true, new CallbackResponsePacket(callbackId)));
        }

        public void SendResponse(ushort callbackId, bool goodResponse, IPacketSerializable packetBuilder)
        {
            sender.Send(new PointerResponsePacket(goodResponse, new CallbackResponsePacket(callbackId, packetBuilder)));
        }
    }
}
