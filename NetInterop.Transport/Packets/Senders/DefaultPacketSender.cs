using System;
using NetInterop.Transport.Core.Abstractions.Packets;
using NetInterop.Transport.Core.Factories;

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

            value.Serialize(packet);

            controller.WriteBlindPacket(packet);
        }

        public void Send(IPacket<TPacket> packet) => controller.WriteBlindPacket(packet);

        public void Send(TPacket packetType, byte[] data)
        {
            var packet = Packet.Create(packetType, data);

            controller.WriteBlindPacket(packet);
        }
    }
}
