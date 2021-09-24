using NetInterop.Extensions;
using NetInterop.Transport.Core.Abstractions.Packets;
using NetInterop.Transport.Core.Packets.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetInterop
{
    public class DefaultPointerReponseHandler : IPacketHandler<PointerOperations>
    {
        private readonly IPointerResponseHandler handler;

        public DefaultPointerReponseHandler(IPointerResponseHandler handler)
        {
            this.handler = handler ?? throw new ArgumentNullException(nameof(handler));
        }

        public PointerOperations PacketType { get; } = PointerOperations.OperationResult;

        public void Handle(IPacket packet)
        {
            PointerResponses response = (PointerResponses)packet.GetByte();

            handler.Handle(response.Contains(PointerResponses.GoodResponse), packet);
        }
    }
}
