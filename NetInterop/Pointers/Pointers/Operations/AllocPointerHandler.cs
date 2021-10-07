using NetInterop.Abstractions;
using NetInterop.Transport.Core.Abstractions.Packets;
using NetInterop.Transport.Core.Packets.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetInterop
{
    public class AllocPointerHandler : IPacketHandler<PointerOperations>
    {
        private readonly IObjectHeap globalHeap;
        private readonly IPointerProvider pointerProvider;
        private readonly IPointerResponseSender sender;

        public AllocPointerHandler(IObjectHeap globalHeap, IPointerProvider pointerProvider, IPointerResponseSender sender)
        {
            this.globalHeap = globalHeap ?? throw new ArgumentNullException(nameof(globalHeap));
            this.pointerProvider = pointerProvider ?? throw new ArgumentNullException(nameof(pointerProvider));
            this.sender = sender ?? throw new ArgumentNullException(nameof(sender));
        }

        public PointerOperations PacketType { get; } = PointerOperations.Alloc;

        public void Handle(IPacket packet)
        {
            ushort callbackId = packet.GetUShort();

            // get the type we should create
            INetPtr typePtr = pointerProvider.Create(packet.GetUShort(), 0);

            // try to get the type and alloc a new object
            try
            {
                INetPtr newPtr = globalHeap.Alloc(typePtr);

                if (newPtr != null)
                {
                    sender.SendResponse(callbackId, true, newPtr);
                    return;
                }
            }
            catch (Exception)
            {
                sender.SendBadResponse(callbackId);
                throw;
            }
        }
    }
}
