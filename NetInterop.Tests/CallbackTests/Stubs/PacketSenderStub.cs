using NetInterop.Transport.Core.Abstractions.Packets;
using NetInterop.Transport.Core.Factories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace NetInterop.Tests.CallbackTests.Stubs
{
    class PacketSenderStub : IPacketSender
    {
        public Queue<IPacket> Sent = new();

        public void Send(IPacketSerializable value)
        {
            var packet = Packet.Create(value.EstimatePacketSize());

            value.Serialize(packet);

            Sent.Enqueue(packet);
        }

        public void Send(IPacket packet)
        {
            Sent.Enqueue(packet);
        }
    }
}
