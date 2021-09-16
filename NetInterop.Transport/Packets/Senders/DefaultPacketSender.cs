using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NetInterop.Transport.Core.Packets;
using NetInterop.Transport.Core.Abstractions;
using NetInterop.Transport.Core.Abstractions.Packets;

namespace NetInterop.Transport.Core.Packets
{
    public class DefaultPacketSender<TPacket> : IPacketSender<TPacket> where TPacket : Enum, IConvertible
    {
        private readonly IPacketController<TPacket> controller;

        public DefaultPacketSender(IPacketController<TPacket> controller)
        {
            this.controller = controller;
        }

        public void Send(IPacketSerializable<TPacket> value)
        {
            int size = value.EstimatePacketSize();

            var packet = Packet.Create(value.PacketType, size);

            value.Serialize(ref packet);

            controller.WriteBlindPacket(packet);
        }

        public void Send(Packet<TPacket> packet) => controller.WriteBlindPacket(packet);

        public void Send(TPacket packetType, Span<byte> data)
        {
            var packet = Packet.Create(packetType, data);

            controller.WriteBlindPacket(packet);
        }
    }
}
