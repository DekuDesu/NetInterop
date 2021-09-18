using NetInterop.Transport.Core.Abstractions.Packets;
using NetInterop.Transport.Core.Packets;
using NetInterop.Transport.Sockets.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetInterop.Transport.Sockets.Packets
{
    /// <summary>
    /// Packet that can be sent and ignored by recipient. This is used to verify the socket is open on the caller's side.
    /// </summary>
    public class ConnectionAliveHandler : IPacketHandler<DefaultPacketTypes>
    {
        public DefaultPacketTypes PacketType { get; } = DefaultPacketTypes.ping;

        public void Handle(IPacket<DefaultPacketTypes> packet) { }
    }
}
