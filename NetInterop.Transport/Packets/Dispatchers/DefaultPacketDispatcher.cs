using NetInterop.Transport.Core.Abstractions.Packets;
using NetInterop.Transport.Core.Packets;
using NetInterop.Transport.Core.Packets.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetInterop.Transport.Core.Packets
{
    public class DefaultPacketDispatcher<TPacketType> : IPacketDispatcher
    {
        private readonly IDictionary<TPacketType, IPacketHandler<TPacketType>> handlers;

        public DefaultPacketDispatcher(IEnumerable<IPacketHandler<TPacketType>> handlers)
        {
            if (handlers is null)
            {
                throw new ArgumentNullException(nameof(handlers));
            }

            this.handlers = handlers.ToDictionary(handler => handler.PacketType);
        }

        public void Dispatch(IPacket packet)
        {
            byte packetType = packet.GetByte();
            handlers[packet].Handle(packet);
        }
    }
}
