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
    class PacketSenderStub<TPacket> : IPacketSender<TPacket> where TPacket : Enum, IConvertible
    {
        public Queue<IPacket<TPacket>> Sent = new();

        public void Send(IPacketSerializable<TPacket> value)
        {
            var packet = Packet.Create((TPacket)(object)0);

            value.Serialize(packet);

            Sent.Enqueue(packet);
        }

        public void Send(IPacket<TPacket> packet)
        {
            Sent.Enqueue(packet);
        }

        public void Send(TPacket packetType, byte[] data)
        {
            var packet = Packet.Create(packetType, data);

            Sent.Enqueue(packet);
        }
    }
}
