using NetInterop.Abstractions;
using NetInterop.Runtime;
using NetInterop.Runtime.MethodHandling;
using NetInterop.Runtime.TypeHandling;
using NetInterop.Tests.CallbackTests.Stubs;
using NetInterop.Transport.Core.Abstractions.Packets;
using NetInterop.Transport.Core.Packets.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using NetInterop.Runtime.Extensions;
using NetInterop.Transport.Core.Factories;

namespace NetInterop.Tests.Runtime
{
    public class RemoteHeapTests
    {
        [Fact]
        public void Test_Alloc()
        {
            var test = new TestObjects();

            var intSerializer = new IntSerializer();
            test.Types.RegisterType<int>((ushort)TypeCode.Int32, () => 0, (ref int num) => { }, intSerializer, intSerializer);

            INetPtr typePtr = test.Types.RegisterType<TestClass>(0x01, () => new TestClass());

            INetPtr<TestClass> ptr = test.RemoteHeap.Create<TestClass>().Result;

            Assert.NotNull(ptr);

            Assert.Equal("0100", ptr.ToString());

            // make sure the object was actually created
            Assert.NotNull(test.Heap.Get(ptr));
        }

        [Fact]
        public void Test_Free()
        {
            var test = new TestObjects();

            var intSerializer = new IntSerializer();
            test.Types.RegisterType<int>((ushort)TypeCode.Int32, () => 0, (ref int num) => { }, intSerializer, intSerializer);

            INetPtr<TestClass> typePtr = test.Types.RegisterType<TestClass>(0x01, () => new TestClass(), (ref TestClass value) => value = null);

            // manually alloc the ptr
            INetPtr<TestClass> ptr = test.Heap.Alloc(typePtr);

            // make sure the object was actually created
            Assert.NotNull(test.Heap.Get(ptr));

            // free the object and check to make sure it was freed
            bool result = test.RemoteHeap.Destroy(ptr).Result;

            Assert.True(result);
            Assert.Null(test.Heap.Get(ptr));
        }

        [Fact]
        public void Test_Set()
        {
            var test = new TestObjects();

            var intSerializer = new IntSerializer();
            INetPtr<int> typePtr = test.Types.RegisterType<int>((ushort)TypeCode.Int32, () => 0, (ref int num) => { }, intSerializer, intSerializer);

            // manually alloc the ptr
            INetPtr<int> ptr = test.Heap.Alloc(typePtr);

            // make sure the object was actually created
            Assert.Equal(0, test.Heap.Get(ptr));

            // free the object and check to make sure it was freed
            bool result = test.RemoteHeap.Set(ptr, 43).Result;

            Assert.True(result);

            Assert.Equal(43, test.Heap.Get(ptr));
        }

        [Fact]
        public void Test_SetAmbiguous()
        {
            var test = new TestObjects();

            var intSerializer = new IntSerializer();
            INetPtr<int> typePtr = test.Types.RegisterType<int>((ushort)TypeCode.Int32, () => 0, (ref int num) => { }, intSerializer, intSerializer);

            // manually alloc the ptr
            INetPtr ptr = test.Heap.Alloc(typePtr);

            // make sure the object was actually created
            Assert.Equal(0, test.Heap.Get(ptr));

            // free the object and check to make sure it was freed
            bool result = test.RemoteHeap.Set(ptr, 43).Result;

            Assert.True(result);

            Assert.Equal(43, test.Heap.Get(ptr));
        }

        [Fact]
        public void Test_Get()
        {
            var test = new TestObjects();

            var intSerializer = new IntSerializer();
            INetPtr<int> typePtr = test.Types.RegisterType<int>((ushort)TypeCode.Int32, () => 0, (ref int num) => { }, intSerializer, intSerializer);

            // manually alloc the ptr
            INetPtr<int> ptr = test.Heap.Alloc(typePtr);

            // make sure the object was actually created
            Assert.Equal(0, test.Heap.Get(ptr));

            // free the object and check to make sure it was freed
            int result = test.RemoteHeap.Get(ptr).Result;

            Assert.Equal(0, result);

            test.Heap.Set(ptr, 23);

            result = test.RemoteHeap.Get(ptr).Result;

            Assert.Equal(23, result);
        }

        [Fact]
        public void Test_GetAmbiguous()
        {
            var test = new TestObjects();

            var intSerializer = new IntSerializer();
            INetPtr<int> typePtr = test.Types.RegisterType<int>((ushort)TypeCode.Int32, () => 0, (ref int num) => { }, intSerializer, intSerializer);

            // manually alloc the ptr
            INetPtr ptr = test.Heap.Alloc(typePtr);

            // make sure the object was actually created
            Assert.Equal(0, test.Heap.Get(ptr));

            // free the object and check to make sure it was freed
            int result = (int)test.RemoteHeap.Get(ptr).Result;

            Assert.Equal(0, result);

            test.Heap.Set(ptr, 23);

            result = (int)test.RemoteHeap.Get(ptr).Result;

            Assert.Equal(23, result);
        }

        private class TestObjects
        {
            public IPacketSerializer<int> IntSerializer { get; set; }
            public IPacketDeserializer<int> IntDeserializer { get; set; }
            public IPointerProvider PointerProvider { get; set; }
            public IType<int> IntNetType { get; set; }
            public ISerializableType<int> SerializableIntNetType { get; set; }
            public ITypeHandler Types { get; set; }
            public IPacketSender Sender { get; set; }
            public IPointerResponseSender PointerSender { get; set; }
            public IPacketHandler<PointerOperations> AllocOperation { get; set; }
            public IPacketHandler<PointerOperations> SetOperation { get; set; }
            public IPacketHandler<PointerOperations> GetOperation { get; set; }
            public IPacketHandler<PointerOperations> FreeOperation { get; set; }
            public IPacketHandler<PointerOperations> InvokeOperation { get; set; }
            public IPacketHandler OperationHandler { get; set; }
            public IDelegateHandler<bool, IPacket> DelegateHandler { get; set; }
            public IPointerResponseHandler PointerCallbackHandler { get; set; }
            public IPacketHandler<PointerOperations> RepsonseHandler { get; set; }
            public IMethodHandler Methods { get; set; }
            public IObjectHeap Heap { get; set; }
            public INetPtr<int> IntTypePointer { get; set; }
            public IRemoteHeap RemoteHeap { get; set; }
            public TestObjects()
            {
                IntSerializer = new DefaultSerializer<int>((num, packet) => packet.AppendInt(num));
                IntDeserializer = new DefaultDeserializer<int>((packet) => packet.GetInt());

                PointerProvider = new DefaultPointerProvider();

                IntTypePointer = PointerProvider.Create(1, 0).As<int>();

                IntNetType = new NetType<int>(IntTypePointer, new DefaultActivator<int>(), new DefaultDeactivator<int>());

                SerializableIntNetType = new SerializableType<int>(IntNetType, IntSerializer, IntDeserializer);

                Types = new NetTypeHandler(PointerProvider);

                Heap = new RuntimeHeap(Types, PointerProvider);

                Sender = new SendAndAutoHandle();

                Methods = new DefaultMethodHandler(PointerProvider, Types, Heap);

                PointerSender = new DefaultPointerResponseSender(Sender);

                DelegateHandler = new DefaultPacketDelegateHandler();

                PointerCallbackHandler = new CallbackPacketHandler(DelegateHandler);

                RemoteHeap = new RemoteHeap(PointerProvider, Sender, Types, Methods, DelegateHandler);

                AllocOperation = new AllocPointerHandler(Heap, PointerProvider, PointerSender);
                SetOperation = new SetPointerHandler(Heap, Types, PointerProvider, PointerSender);
                GetOperation = new GetPointerHandler(Heap, Types, PointerProvider, PointerSender);
                FreeOperation = new FreePointerHandler(Heap, PointerProvider, PointerSender);
                InvokeOperation = new InvokePointerHandler(PointerProvider, PointerSender, Methods);
                RepsonseHandler = new DefaultPointerReponseHandler(PointerCallbackHandler);

                OperationHandler = new PointerPacketDispatchHandler(RepsonseHandler, AllocOperation, SetOperation, GetOperation, FreeOperation, InvokeOperation);

                ((SendAndAutoHandle)Sender).Handler = OperationHandler;
            }
        }
        public class TestClass
        {
            public bool RanTest { get; set; } = false;
            public bool RanAdder { get; set; } = false;

            public void Test()
            {
                RanTest = true;
            }

            public int Adder(int a, int b)
            {
                RanAdder = true;
                return a + b;
            }

            public static void StaticVoid()
            {

            }
            public static int GetInt()
            {
                return 23;
            }
            public static int StaticAdder(int a, int b)
            {
                return a + b;
            }
        }

        public class IntSerializer : IPacketSerializer<int>, IPacketDeserializer<int>
        {
            public object AmbiguousDeserialize(IPacket packet)
            {
                return packet.GetInt();
            }

            public int Deserialize(IPacket packet)
            {
                return packet.GetInt();
            }

            public void Serialize(int value, IPacket packetBuilder)
            {
                packetBuilder.AppendInt(value);
            }
        }
        private class SendAndAutoHandle : IPacketSender
        {
            public IPacketHandler Handler { get; set; }

            public void Send(IPacketSerializable value)
            {
                var packet = Packet.Create(value.EstimatePacketSize());
                value.Serialize(packet);
                Handler.Handle(packet);
            }
        }
    }
}
