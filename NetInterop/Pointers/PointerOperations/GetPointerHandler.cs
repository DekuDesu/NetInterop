using NetInterop.Transport.Core.Abstractions.Packets;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetInterop
{
    public class GetPointerHandler : IPacketHandler<PointerOperations>
    {
        private readonly INetworkTypeHandler typeHandler;
        private readonly IPointerProvider pointerProvider;
        private readonly IPointerResponseSender sender;

        public GetPointerHandler(INetworkTypeHandler typeHandler, IPointerProvider pointerProvider, IPointerResponseSender sender)
        {
            this.typeHandler = typeHandler ?? throw new ArgumentNullException(nameof(typeHandler));
            this.pointerProvider = pointerProvider ?? throw new ArgumentNullException(nameof(pointerProvider));
            this.sender = sender ?? throw new ArgumentNullException(nameof(sender));
        }

        public PointerOperations PacketType { get; } = PointerOperations.Set;

        public void Handle(IPacket packet)
        {
            // get the type we should create
            INetPtr ptr = pointerProvider.Deserialize(packet);

            if (ptr is null)
            {
                sender.SendBadResponse();
            }

            // try to get the type and alloc a new object
            if (typeHandler.TryGetAmbiguousSerializableType(ptr, out var type))
            {
                object value = type.GetPtr(ptr);

                if (value is null)
                {
                    sender.SendBadResponse();
                }

                sender.SendResponse(true, new PacketSerializerProxy(value, type));
            }

            sender.SendBadResponse();
        }
    }
}
