using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using NetInterop;
using NetInterop.Runtime.MethodHandling;
using NetInterop.Transport.Core.Abstractions.Packets;
using NetInterop.Transport.Core.Packets.Extensions;
using System.Reflection;
using NetInterop.Transport.Core.Factories;
using NetInterop.Runtime.Extensions;
using NetInterop.Abstractions;
using NetInterop.Runtime;
using NetInterop.Runtime.TypeHandling;

namespace NetInterop.Tests.MethodHandlerTests
{
    public class MethodHandlerTests
    {
        [Fact]
        public void Test_Registration()
        {
            IPointerProvider pointerProvider = new DefaultPointerProvider();
            ITypeHandler typeHandler = new NetTypeHandler(pointerProvider);
            IObjectHeap heap = new RuntimeHeap(typeHandler, pointerProvider);
            IMethodHandler methodHandler = new DefaultMethodHandler(pointerProvider, typeHandler, heap);
            var intSerializer = new IntSerializer();

            typeHandler.RegisterType<TestClass>(0x01);

            typeHandler.RegisterType<int>((ushort)TypeCode.Int32, () => 0, (ref int num) => { }, intSerializer, intSerializer);

            INetPtr methodPtr = methodHandler.Register(typeof(TestClass).GetMethod(nameof(TestClass.Test)));

            Assert.Equal(1, methodPtr.PtrAddress);

            methodPtr = methodHandler.Register(typeof(TestClass).GetMethod(nameof(TestClass.Adder)));

            Assert.Equal(2, methodPtr.PtrAddress);

            methodPtr = methodHandler.Register(typeof(TestClass).GetMethod(nameof(TestClass.GetInt)));

            Assert.Equal(3, methodPtr.PtrAddress);

            methodPtr = methodHandler.Register(typeof(TestClass).GetMethod(nameof(TestClass.StaticAdder)));

            Assert.Equal(4, methodPtr.PtrAddress);
        }

        [Fact]
        public void Test_ParameterLessReturnless()
        {
            IPointerProvider pointerProvider = new DefaultPointerProvider();
            ITypeHandler typeHandler = new NetTypeHandler(pointerProvider);

            IObjectHeap heap = new RuntimeHeap(typeHandler, pointerProvider);

            IMethodHandler methodHandler = new DefaultMethodHandler(pointerProvider, typeHandler, heap);
            var intSerializer = new IntSerializer();

            INetPtr typePtr = typeHandler.RegisterType<TestClass>(0x01, () => new TestClass());

            typeHandler.RegisterType<int>((ushort)TypeCode.Int32, () => 0, (ref int num) => { }, intSerializer, intSerializer);

            INetPtr methodPtr = methodHandler.Register(typeof(TestClass).GetMethod(nameof(TestClass.Test)));

            Assert.Equal(1, methodPtr.PtrAddress);

            // create an instance to call the method on
            INetPtr<TestClass> instancePtr = heap.Alloc(typePtr).As<TestClass>();

            IPacket inputPacket = Packet.Create(instancePtr.EstimatePacketSize());

            // append the instance ptr to the packet so we know which instance to invoke the method on
            inputPacket.AppendSerializable(instancePtr);

            IPacket outputPacket = Packet.Create(0);

            methodHandler.Invoke(methodPtr, inputPacket, outputPacket);

            // get the instance value and make sure RanTest is tru
            TestClass instance = heap.Get(instancePtr);

            Assert.True(instance.RanTest);
        }

        [Fact]
        public void Test_Static_ParameterLessReturnless()
        {
            IPointerProvider pointerProvider = new DefaultPointerProvider();
            ITypeHandler typeHandler = new NetTypeHandler(pointerProvider);
            IObjectHeap heap = new RuntimeHeap(typeHandler, pointerProvider);
            IMethodHandler methodHandler = new DefaultMethodHandler(pointerProvider, typeHandler, heap);
            var intSerializer = new IntSerializer();

            INetPtr typePtr = typeHandler.RegisterType<TestClass>(0x01, () => new TestClass());

            Assert.True(typeHandler.TryGetType<TestClass>(out IType<TestClass> networkType));

            typeHandler.RegisterType<int>((ushort)TypeCode.Int32, () => 0, (ref int num) => { }, intSerializer, intSerializer);

            INetPtr methodPtr = methodHandler.Register(typeof(TestClass).GetMethod(nameof(TestClass.GetInt)));

            Assert.Equal(1, methodPtr.PtrAddress);

            // blank packet, no instance ptr
            IPacket inputPacket = Packet.Create(0);

            IPacket outputPacket = Packet.Create(0);

            methodHandler.Invoke(methodPtr, inputPacket, outputPacket);

            Assert.Equal(23, outputPacket.GetInt());
        }

        [Fact]
        public void Test_Instance_Adder_NoReturn()
        {
            IPointerProvider pointerProvider = new DefaultPointerProvider();
            ITypeHandler typeHandler = new NetTypeHandler(pointerProvider);

            IObjectHeap heap = new RuntimeHeap(typeHandler, pointerProvider);

            IMethodHandler methodHandler = new DefaultMethodHandler(pointerProvider, typeHandler, heap);
            var intSerializer = new IntSerializer();

            INetPtr typePtr = typeHandler.RegisterType<TestClass>(0x01, () => new TestClass());

            Assert.True(typeHandler.TryGetType<TestClass>(out IType<TestClass> networkType));

            typeHandler.RegisterType<int>((ushort)TypeCode.Int32, () => 0, (ref int num) => { }, intSerializer, intSerializer);

            INetPtr methodPtr = methodHandler.Register(typeof(TestClass).GetMethod(nameof(TestClass.Adder)));

            Assert.Equal(1, methodPtr.PtrAddress);

            // create an instance to call the method on
            INetPtr<TestClass> instancePtr = heap.Alloc(typePtr).As<TestClass>();

            IPacket inputPacket = Packet.Create(0);

            // append the instance ptr to the packet so we know which instance to invoke the method on
            inputPacket.AppendSerializable(instancePtr);

            inputPacket.AppendInt(12);

            inputPacket.AppendInt(12);

            IPacket outputPacket = Packet.Create(0);

            methodHandler.Invoke(methodPtr, inputPacket);

            // get the instance value and make sure RanTest is tru
            TestClass instance = (TestClass)heap.Get(instancePtr);

            Assert.True(instance.RanAdder);
        }

        [Fact]
        public void Test_Instance_Adder_ReturnValue()
        {
            IPointerProvider pointerProvider = new DefaultPointerProvider();

            ITypeHandler typeHandler = new NetTypeHandler(pointerProvider);

            INetPtr typePtr = typeHandler.RegisterType<TestClass>(0x01, () => new TestClass());

            var intSerializer = new IntSerializer();
            typeHandler.RegisterType<int>((ushort)TypeCode.Int32, () => 0, (ref int num) => { }, intSerializer, intSerializer);

            Assert.True(typeHandler.TryGetType<TestClass>(out IType<TestClass> networkType));

            IObjectHeap<TestClass> heap = new ObjectHeap<TestClass>(networkType, pointerProvider);

            IMethodHandler methodHandler = new DefaultMethodHandler(pointerProvider, typeHandler, heap);


            INetPtr methodPtr = methodHandler.Register(typeof(TestClass).GetMethod(nameof(TestClass.Adder)));

            Assert.Equal(1, methodPtr.PtrAddress);

            // create an instance to call the method on
            INetPtr<TestClass> instancePtr = heap.Alloc(typePtr).As<TestClass>();

            IPacket inputPacket = Packet.Create(0);

            // append the instance ptr to the packet so we know which instance to invoke the method on
            inputPacket.AppendSerializable(instancePtr);

            inputPacket.AppendInt(12);

            inputPacket.AppendInt(12);

            IPacket outputPacket = Packet.Create(0);

            methodHandler.Invoke(methodPtr, inputPacket, outputPacket);

            // get the instance value and make sure RanTest is tru
            TestClass instance = heap.Get(instancePtr);

            Assert.True(instance.RanAdder);

            Assert.Equal(24, outputPacket.GetInt());
        }

        [Fact]
        public void Test_Static_Adder_ReturnValue()
        {
            IPointerProvider pointerProvider = new DefaultPointerProvider();
            ITypeHandler typeHandler = new NetTypeHandler(pointerProvider);
            IObjectHeap heap = new RuntimeHeap(typeHandler, pointerProvider);
            IMethodHandler methodHandler = new DefaultMethodHandler(pointerProvider, typeHandler, heap);
            var intSerializer = new IntSerializer();

            INetPtr typePtr = typeHandler.RegisterType<TestClass>(0x01, () => new TestClass());

            Assert.True(typeHandler.TryGetType<TestClass>(out IType<TestClass> networkType));

            typeHandler.RegisterType<int>((ushort)TypeCode.Int32, () => 0, (ref int num) => { }, intSerializer, intSerializer);

            INetPtr methodPtr = methodHandler.Register(typeof(TestClass).GetMethod(nameof(TestClass.StaticAdder)));

            Assert.Equal(1, methodPtr.PtrAddress);

            IPacket inputPacket = Packet.Create(0);

            // dont append an instance ptr, only append the parameters
            inputPacket.AppendInt(12);

            inputPacket.AppendInt(12);

            IPacket outputPacket = Packet.Create(0);

            methodHandler.Invoke(methodPtr, inputPacket, outputPacket);

            Assert.Equal(24, outputPacket.GetInt());
        }

        [Fact]
        public void Test_RegistrationOverloads()
        {
            var test = new TestObjects();

            var intSerializer = new IntSerializer();

            INetPtr<TestClass> typePtr = test.Types.RegisterType<TestClass>(0x01, () => new TestClass());

            Assert.True(test.Types.TryGetType<TestClass>(out IType<TestClass> networkType));

            INetPtr<int> intTypePtr = test.Types.RegisterType<int>((ushort)TypeCode.Int32, () => 0, (ref int num) => { }, intSerializer, intSerializer);

            // test that we receive net ptrs with correct types

            // should bev void(no type)
            INetPtr ptr = test.Methods.Register(TestClass.StaticVoid);

            Assert.Equal(typeof(NetPtr), ptr.GetType());
            Assert.NotEqual(typeof(NetPtr<>), ptr.GetType());

            // should be of the same type as the return type of the provided istance method
            INetPtr intPtr = test.Methods.Register<TestClass, int, int, int>(x => x.Adder);

            Assert.Equal(typeof(NetPtr<int>), intPtr.GetType());

            // we should get a net ptr that has a generic type of the return type
            // regarless of us specifying it as a generic param
            INetPtr otherIntPtr = test.Methods.Register(typeof(MethodHandlerTests).GetMethod(nameof(StaticIntDelegate)));

            Assert.Equal(typeof(NetPtr<int>), otherIntPtr.GetType());
        }

        [Fact]
        public void Test_DuplicateRegistrations()
        {
            var test = new TestObjects();

            var intSerializer = new IntSerializer();

            INetPtr<TestClass> typePtr = test.Types.RegisterType<TestClass>(0x01, () => new TestClass());

            Assert.True(test.Types.TryGetType<TestClass>(out IType<TestClass> networkType));

            INetPtr<int> intTypePtr = test.Types.RegisterType<int>((ushort)TypeCode.Int32, () => 0, (ref int num) => { }, intSerializer, intSerializer);

            // any duplicate registrations to the method handler should return the same ptr each time
            void ShouldNotThrow()
            {
                INetPtr<int> ptr = test.Methods.Register<int>(typeof(MethodHandlerTests).GetMethod(nameof(StaticIntDelegate)));
                throw new OperationCanceledException();
            }
            void ShouldThrow()
            {
                INetPtr<float> ptr = test.Methods.Register<float>(typeof(MethodHandlerTests).GetMethod(nameof(StaticIntDelegate)));
            }
            Assert.Throws<OperationCanceledException>(ShouldNotThrow);
            Assert.Throws<InvalidCastException>(ShouldThrow);
        }

        [Fact]
        public void Test_RegistrationThrowsWhenWrongGenericType()
        {
            var test = new TestObjects();

            var intSerializer = new IntSerializer();

            INetPtr<TestClass> typePtr = test.Types.RegisterType<TestClass>(0x01, () => new TestClass());

            Assert.True(test.Types.TryGetType<TestClass>(out IType<TestClass> networkType));

            INetPtr<int> intTypePtr = test.Types.RegisterType<int>((ushort)TypeCode.Int32, () => 0, (ref int num) => { }, intSerializer, intSerializer);

            // any duplicate registrations to the method handler should return the same ptr each time
            INetPtr<int> ptr = test.Methods.Register(StaticIntDelegate);

            Assert.Equal(ptr, test.Methods.Register(StaticIntDelegate));
            Assert.Equal(ptr, test.Methods.Register(StaticIntDelegate));
            Assert.Equal(ptr, test.Methods.Register(StaticIntDelegate));
        }

        public static int StaticIntDelegate()
        {
            return default;
        }

        public class TestObjects
        {
            public IPointerProvider PointerProvider { get; set; } = new DefaultPointerProvider();
            public ITypeHandler Types { get; set; }
            public IObjectHeap Heap { get; set; }
            public IMethodHandler Methods { get; set; }
            public TestObjects()
            {
                PointerProvider = new DefaultPointerProvider();
                Types = new NetTypeHandler(PointerProvider);
                Heap = new RuntimeHeap(Types, PointerProvider);
                Methods = new DefaultMethodHandler(PointerProvider, Types, Heap);

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

            public int EstimatePacketSize(int value) => sizeof(int);

            public void Serialize(int value, IPacket packetBuilder)
            {
                packetBuilder.AppendInt(value);
            }
        }
    }
}
