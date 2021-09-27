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
        private readonly Func<byte, TPacketType> keyConverter;
        public DefaultPacketDispatcher(IEnumerable<IPacketHandler<TPacketType>> handlers, Func<byte, TPacketType> keyConverter)
        {
            this.handlers = handlers?.ToDictionary(handler => handler.PacketType) ?? throw new ArgumentNullException(nameof(handlers));
            this.keyConverter = keyConverter ?? throw new ArgumentNullException(nameof(keyConverter));
        }

        public void Dispatch(IPacket packet)
        {
            TPacketType type = keyConverter(packet.GetByte());
            handlers[type].Handle(packet);
        }
    }
}
