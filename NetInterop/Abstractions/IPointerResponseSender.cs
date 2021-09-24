using NetInterop.Transport.Core.Abstractions.Packets;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetInterop
{
    public interface IPointerResponseSender
    {
        void SendBadResponse(ushort callbackId);
        void SendGoodResponse(ushort callbackId);
        void SendResponse(ushort callbackId, bool goodResponse, IPacketSerializable packetBuilder);
    }
}
