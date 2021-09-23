using NetInterop.Transport.Core.Abstractions.Packets;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetInterop
{
    public class SetPointerHandler : IPacketHandler<PointerOperations>
    {
        private readonly INetworkTypeHandler typeHandler;
        private readonly IPointerProvider pointerProvider;
        private readonly IPointerResponseSender sender;

        public SetPointerHandler(INetworkTypeHandler typeHandler, IPointerProvider pointerProvider, IPointerResponseSender sender)
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
                object value = type.AmbiguousDeserialize(packet);

                if (value is null)
                {
                    sender.SendBadResponse();
                }

                type.SetPtr(ptr, value);

                sender.SendGoodResponse();
            }

            sender.SendBadResponse();
        }
    }
}
