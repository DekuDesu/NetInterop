using NetInterop.Transport.Core.Abstractions.Packets;
using NetInterop.Transport.Core.Packets;
using NetInterop.Transport.Core.Packets.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using NetInterop.Transport.Core.Factories;
namespace RemoteInvokeTests.Abstractions
{
    public class PacketDeserializerTests
    {
        [Fact]
        public void Test_Serialize()
        {
            MyClass value = new() { Value = 12 };

            IPacket packet = Packet.Create(4);

            value.Serialize(packet);

            Assert.Equal(12, packet.GetInt());
        }

        [Fact]
        public void Test_Deserialize()
        {
            MyClass value = new() { Value = 12 };

            IPacket packet = Packet.Create(4);

            packet.AppendInt(12);

            Assert.Equal(12, value.Deserialize(packet));
        }

        [Fact]
        public void Test_NestedSerialization()
        {
            MyClass value = new() { Value = 12 };

            IPacket packet = Packet.Create(4);

            value.Serialize(packet);

            Assert.Equal(12, packet.GetInt());
            Assert.Equal(69, packet.GetInt());
        }

        [Fact]
        public void Test_NestedDeserialization()
        {
            MyClass value = new() { Value = 12 };

            IPacket packet = Packet.Create(0);

            value.Serialize(packet);

            int[] deserialized = ((IPacketDeserializable<int[]>)Activator.CreateInstance(value.GetType())).Deserialize(packet);

            Assert.Equal(12, deserialized[0]);
            Assert.Equal(69, deserialized[1]);
        }

        public enum TestPacketTypes
        {
            none,
            Pass,
            Fail
        }
        public class OtherClass : IPacketSerializable, IPacketDeserializable<int>
        {
            public int Value { get; set; } = 69;
            public TestPacketTypes PacketType { get; }

            public int Deserialize(IPacket packet)
            {
                return packet.GetInt();
            }

            public int EstimatePacketSize()
            {
                throw new NotImplementedException();
            }

            public void Serialize(IPacket builder)
            {
                builder.AppendInt(Value);
            }
        }
        public class MyClass : IPacketDeserializable<int>, IPacketSerializable, IPacketDeserializable<int[]>
        {
            public int Value { get; set; }
            public TestPacketTypes PacketType { get; }

            private readonly OtherClass other = new();

            public int Deserialize(IPacket packet)
            {
                return packet.GetInt();
            }

            public void Serialize(IPacket builder)
            {
                builder.AppendInt(Value);
                builder.AppendSerializable(other);
            }

            int[] IPacketDeserializable<int[]>.Deserialize(IPacket packet)
            {
                return new int[] { packet.GetInt(), packet.GetInt() };
            }

            public int EstimatePacketSize()
            {
                throw new NotImplementedException();
            }
        }
    }
}
