using NetInterop.Abstractions;
using NetInterop.Transport.Core.Abstractions.Packets;
using NetInterop.Transport.Core.Packets.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetInterop
{
    public class SetPointerHandler : IPacketHandler<PointerOperations>
    {
        private readonly IObjectHeap heap;
        private readonly ITypeHander typeHandler;
        private readonly IPointerProvider pointerProvider;
        private readonly IPointerResponseSender sender;

        public SetPointerHandler(IObjectHeap heap, ITypeHander typeHandler, IPointerProvider pointerProvider, IPointerResponseSender sender)
        {
            this.heap = heap ?? throw new ArgumentNullException(nameof(heap));
            this.pointerProvider = pointerProvider ?? throw new ArgumentNullException(nameof(pointerProvider));
            this.sender = sender ?? throw new ArgumentNullException(nameof(sender));
            this.typeHandler = typeHandler ?? throw new ArgumentNullException(nameof(typeHandler));
        }

        public PointerOperations PacketType { get; } = PointerOperations.Set;

        public void Handle(IPacket packet)
        {
            ushort callbackId = packet.GetUShort();

            // get the type we should create
            INetPtr ptr = pointerProvider.Deserialize(packet);

            if (ptr is null)
            {
                sender.SendBadResponse(callbackId);
            }

            try
            {
                // try to get the type and alloc a new object
                if (typeHandler.TryGetSerializableType(ptr, out var type))
                {
                    object value = type.AmbiguousDeserialize(packet);

                    if (value != null)
                    {
                        heap.Set(ptr, value);

                        sender.SendGoodResponse(callbackId);

                        return;
                    }
                }
                sender.SendBadResponse(callbackId);
            }
            catch (Exception)
            {
                sender.SendBadResponse(callbackId);

                throw;
            }
        }
    }
}
