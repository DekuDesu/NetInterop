using NetInterop.Transport.Core.Abstractions.Packets;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetInterop
{
    public interface IPointerResponseSender
    {
        void SendBadResponse();
        void SendGoodResponse();
        void SendResponse(bool goodResponse, IPacketSerializable packetBuilder);
    }
}
