using NetInterop.Transport.Core.Abstractions.Packets;
using NetInterop.Transport.Core.Packets.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetInterop
{
    public class AllocPointerHandler : IPacketHandler<PointerOperations>
    {
        private readonly INetworkTypeHandler typeHandler;
        private readonly IPointerProvider pointerProvider;
        private readonly IPointerResponseSender sender;

        public AllocPointerHandler(INetworkTypeHandler typeHandler, IPointerProvider pointerProvider, IPointerResponseSender sender)
        {
            this.typeHandler = typeHandler ?? throw new ArgumentNullException(nameof(typeHandler));
            this.pointerProvider = pointerProvider ?? throw new ArgumentNullException(nameof(pointerProvider));
            this.sender = sender ?? throw new ArgumentNullException(nameof(sender));
        }

        public PointerOperations PacketType { get; } = PointerOperations.Alloc;

        public void Handle(IPacket packet)
        {
            // get the type we should create
            INetPtr typePtr = pointerProvider.Create(packet.GetUShort(), 0);

            // try to get the type and alloc a new object
            if (typeHandler.TryGetAmbiguousType(typePtr, out var type))
            {
                INetPtr newPtr = type.AllocPtr();

                if (newPtr != null)
                {
                    sender.SendResponse(true, newPtr);
                }
            }
            sender.SendBadResponse();
        }
    }
}
