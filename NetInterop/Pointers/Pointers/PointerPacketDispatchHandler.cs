using NetInterop.Transport.Core.Abstractions.Packets;
using NetInterop.Transport.Core.Packets.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetInterop
{
    /// <summary>
    /// Dispatches pointer operations packets to individual handlers, pointer packets are wrapped with PointerOperationPackets
    /// </summary>
    /// <typeparam name="TPacket"></typeparam>
    public class PointerPacketDispatchHandler : IPacketHandler
    {
        private readonly IDictionary<PointerOperations, IPacketHandler<PointerOperations>> handlers;

        public PointerPacketDispatchHandler(params IPacketHandler<PointerOperations>[] handlers)
        {
            this.handlers = handlers.ToDictionary(x => x.PacketType);
        }

        public void Handle(IPacket packet)
        {
            PointerOperations operation = (PointerOperations)packet.GetByte();

            handlers[operation].Handle(packet);
        }
    }
}
