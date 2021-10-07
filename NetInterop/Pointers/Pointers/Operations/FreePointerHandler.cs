using NetInterop.Abstractions;
using NetInterop.Transport.Core.Abstractions.Packets;
using NetInterop.Transport.Core.Packets.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetInterop
{
    public class FreePointerHandler : IPacketHandler<PointerOperations>
    {
        private readonly IObjectHeap heap;
        private readonly IPointerProvider pointerProvider;
        private readonly IPointerResponseSender sender;

        public FreePointerHandler(IObjectHeap heap, IPointerProvider pointerProvider, IPointerResponseSender sender)
        {
            this.heap = heap ?? throw new ArgumentNullException(nameof(heap));
            this.pointerProvider = pointerProvider ?? throw new ArgumentNullException(nameof(pointerProvider));
            this.sender = sender ?? throw new ArgumentNullException(nameof(sender));
        }

        public PointerOperations PacketType { get; } = PointerOperations.Free;

        public void Handle(IPacket packet)
        {
            ushort callbackId = packet.GetUShort();

            // get the type we should create
            INetPtr ptr = pointerProvider.Deserialize(packet);

            if (ptr != null)
            {
                try
                {
                    heap.Free(ptr);
                    sender.SendGoodResponse(callbackId);
                }
                catch (Exception)
                {
                    sender.SendBadResponse(callbackId);
                    throw;
                }
            }
        }
    }
}
