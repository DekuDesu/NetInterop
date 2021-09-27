using System;
using NetInterop.Transport.Core.Abstractions.Packets;
using NetInterop.Transport.Core.Factories;

namespace NetInterop.Transport.Core.Packets
{
    public class DefaultPacketSender : IPacketSender
    {
        private readonly IPacketController controller;

        public DefaultPacketSender(IPacketController controller)
        {
            this.controller = controller;
        }

        public void Send(IPacketSerializable value)
        {
            int size = value.EstimatePacketSize();

            var packet = Packet.Create(size);

            value.Serialize(packet);

            controller.WritePacket(packet);
        }

        public void Send(IPacket packet) => controller.WritePacket(packet);
    }
}
