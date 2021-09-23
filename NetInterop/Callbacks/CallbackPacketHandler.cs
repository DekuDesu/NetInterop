using NetInterop.Transport.Core.Abstractions.Packets;
using NetInterop.Transport.Core.Packets.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetInterop.Callbacks
{
    public class CallbackPacketHandler : IPointerResponseHandler
    {
        private readonly IDelegateHandler<bool, IPacket> delegatehandler;

        public CallbackPacketHandler(IDelegateHandler<bool, IPacket> delegatehandler)
        {
            this.delegatehandler = delegatehandler;
        }

        public void Handle(bool goodResponse, IPacket packet)
        {
            ushort callbackId = packet.GetUShort();

            delegatehandler.Invoke(callbackId, goodResponse, packet);
        }
    }
}
