﻿using NetInterop.Transport.Core.Abstractions.Packets;
using NetInterop.Transport.Core.Packets.Extensions;
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

        public PointerOperations PacketType { get; } = PointerOperations.Get;

        public void Handle(IPacket packet)
        {
            ushort callbackId = packet.GetUShort();
            // get the type we should create
            INetPtr ptr = pointerProvider.Deserialize(packet);

            if (ptr is null)
            {
                sender.SendBadResponse(callbackId);
            }

            // try to get the type and alloc a new object
            if (typeHandler.TryGetAmbiguousSerializableType(ptr, out var type))
            {
                object value = type.GetPtr(ptr);

                if (value != null)
                {
                    sender.SendResponse(callbackId, true, new PacketSerializerProxy(value, type));
                    return;
                }
            }
            sender.SendBadResponse(callbackId);
        }
    }
}
