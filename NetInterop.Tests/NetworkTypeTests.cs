using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using NetInterop;
using NetInterop.Attributes;
using NetInterop.Transport.Core.Abstractions.Packets;
using NetInterop.Transport.Core.Packets.Extensions;
using NetInterop.Transport.Core.Factories;
using NetInterop.Runtime.Extensions;
using NetInterop.Abstractions;
using NetInterop.Runtime;

namespace NetInterop.Tests
{
    public class NetworkTypeTests
    {
        [Fact]
        public void Test_BasicRegister()
        {
            IPointerProvider pointerProvider = new DefaultPointerProvider();
            INetTypeHandler typeHandler = new NetTypeHandler(pointerProvider);

            typeHandler.RegisterType<int>(1);

            Assert.True(typeHandler.TryGetType<int>(out var netType));

            Assert.NotNull(netType);

            Assert.Equal(1, netType.TypePointer.PtrType);
        }

        [Fact]
        public void Test_GetTypePtr()
        {
            IPointerProvider pointerProvider = new DefaultPointerProvider();
            INetTypeHandler typeHandler = new NetTypeHandler(pointerProvider);

            typeHandler.RegisterType<int>(1);

            Assert.True(typeHandler.TryGetType<int>(out var netType));

            Assert.NotNull(netType);

            Assert.Equal(1, netType.TypePointer.PtrType);
        }

        [Fact]
        public void Test_InstantiatorAndDisposerRegister()
        {
            IPointerProvider pointerProvider = new DefaultPointerProvider();
            INetTypeHandler typeHandler = new NetTypeHandler(pointerProvider);

            bool instantiated = false; ;
            bool disposed = false;

            int Instantiator()
            {
                instantiated = true;
                return 12;
            }
            void Disposer(int value)
            {
                disposed = true;
                _ = value;
            }

            typeHandler.RegisterType<int>(1, Instantiator, Disposer);

            Assert.True(typeHandler.TryGetType<int>(out var netType));

            IObjectHeap<int> heap = new ObjectHeap<int>(netType, pointerProvider);

            INetPtr<int> ptr = heap.Alloc(null).As<int>();

            Assert.True(instantiated);

            heap.Free(ptr);

            Assert.True(disposed);
        }

        [Fact]
        public void Test_Serialization()
        {
            IPointerProvider pointerProvider = new DefaultPointerProvider();
            INetTypeHandler typeHandler = new NetTypeHandler(pointerProvider);

            var serializer = new TestSerializer();

            typeHandler.RegisterType<TestSerializableClass>(0x01, serializer, serializer);

            Assert.True(typeHandler.TryGetSerializableType(pointerProvider.Create(0x01, 0), out var netType));

            IPacket packet = Packet.Create(0);

            packet.AppendInt(32);

            object result = netType.AmbiguousDeserialize(packet);

            Assert.Equal(32, ((TestSerializableClass)result).Value);

            packet = Packet.Create(0);

            TestSerializableClass newClass = new() { Value = 43 };

            netType.AmbiguousSerialize(newClass, packet);

            Assert.Equal(43, packet.GetInt());
        }

        private class TestSerializer : IPacketSerializer<TestSerializableClass>, IPacketDeserializer<TestSerializableClass>
        {
            public object AmbiguousDeserialize(IPacket packet)
            {
                return new TestSerializableClass() { Value = packet.GetInt() };
            }

            public TestSerializableClass Deserialize(IPacket packet)
            {
                return new TestSerializableClass() { Value = packet.GetInt() };
            }

            public void Serialize(TestSerializableClass value, IPacket packetBuilder)
            {
                packetBuilder.AppendInt(value.Value);
            }
        }

        [InteropId(0x01)]
        private class TestSerializableClass
        {
            public int Value { get; set; }
        }
    }
}
