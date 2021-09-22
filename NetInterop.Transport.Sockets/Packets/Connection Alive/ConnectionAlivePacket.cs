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
    public class ConnectionAlivePacket : IPacketSerializable<DefaultPacketTypes>
    {
        public DefaultPacketTypes PacketType { get; } = DefaultPacketTypes.ping;

        public int EstimatePacketSize() => 0;

        public void Serialize(IPacket packetBuilder) { _ = packetBuilder; }
    }
}
