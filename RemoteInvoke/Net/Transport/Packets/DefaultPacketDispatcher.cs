using RemoteInvoke.Net.Transport.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemoteInvoke.Net.Transport.Packets
{
    public class DefaultPacketDispatcher<TPacketType> : IPacketDispatcher<TPacketType> where TPacketType : Enum, IConvertible
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

        public void Dispatch(ref Packet<TPacketType> packet)
        {
            handlers[packet.PacketType].Handle(ref packet);
        }
    }
}
