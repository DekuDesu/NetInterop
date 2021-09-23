using NetInterop.Transport.Core.Abstractions.Packets;
using NetInterop.Transport.Core.Packets.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetInterop
{
    /// <summary>
    /// Dispatches pointer operations packets to individual handlers
    /// </summary>
    /// <typeparam name="TPacket"></typeparam>
    public class PointerPacketDispatchHandler<TPacket> : IPacketHandler<TPacket> where TPacket : Enum, IConvertible
    {
        private readonly IDictionary<PointerOperations, IPacketHandler<PointerOperations>> handlers;

        public PointerPacketDispatchHandler(IEnumerable<IPacketHandler<PointerOperations>> handlers)
        {
            this.handlers = handlers.ToDictionary(x => x.PacketType);
        }

        // the 0th value in any user-defned packet type should be reserved for net interop pointer operations
        public TPacket PacketType { get; } = default;

        public void Handle(IPacket packet)
        {
            PointerOperations operation = (PointerOperations)packet.GetByte();

            handlers[operation].Handle(packet);
        }
    }
}
