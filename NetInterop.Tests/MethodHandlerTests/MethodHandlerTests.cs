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

namespace NetInterop.Tests.MethodHandlerTests
{
    public class MethodHandlerTests
    {
        [Fact]
        public void Test_Registration()
        {
            IPointerProvider pointerProvider = new DefaultPointerProvider();
            INetworkTypeHandler typeHandler = new DefaultNetworkTypeHandler(pointerProvider);
            INetworkMethodHandler methodHandler = new DefaultMethodHandler(pointerProvider, typeHandler);
            var intSerializer = new IntSerializer();

            typeHandler.RegisterType<TestClass>(0x01);

            typeHandler.RegisterType<int>((ushort)TypeCode.Int32, () => 0, (num) => { }, intSerializer, intSerializer);

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
            INetworkTypeHandler typeHandler = new DefaultNetworkTypeHandler(pointerProvider);
            INetworkMethodHandler methodHandler = new DefaultMethodHandler(pointerProvider, typeHandler);
            var intSerializer = new IntSerializer();

            INetPtr typePtr = typeHandler.RegisterType<TestClass>(0x01, () => new TestClass());

            Assert.True(typeHandler.TryGetType<TestClass>(out INetworkType<TestClass> networkType));

            typeHandler.RegisterType<int>((ushort)TypeCode.Int32, () => 0, (num) => { }, intSerializer, intSerializer);

            INetPtr methodPtr = methodHandler.Register(typeof(TestClass).GetMethod(nameof(TestClass.Test)));

            Assert.Equal(1, methodPtr.PtrAddress);

            // create an instance to call the method on
            INetPtr<TestClass> instancePtr = networkType.AllocPtr().As<TestClass>();

            IPacket inputPacket = Packet.Create(instancePtr.EstimatePacketSize());

            // append the instance ptr to the packet so we know which instance to invoke the method on
            inputPacket.AppendSerializable(instancePtr);

            IPacket outputPacket = Packet.Create(0);

            methodHandler.Invoke(methodPtr, inputPacket, outputPacket);

            // get the instance value and make sure RanTest is tru
            TestClass instance = networkType.GetPtr(instancePtr);

            Assert.True(instance.RanTest);
        }

        [Fact]
        public void Test_Static_ParameterLessReturnless()
        {
            IPointerProvider pointerProvider = new DefaultPointerProvider();
            INetworkTypeHandler typeHandler = new DefaultNetworkTypeHandler(pointerProvider);
            INetworkMethodHandler methodHandler = new DefaultMethodHandler(pointerProvider, typeHandler);
            var intSerializer = new IntSerializer();

            INetPtr typePtr = typeHandler.RegisterType<TestClass>(0x01, () => new TestClass());

            Assert.True(typeHandler.TryGetType<TestClass>(out INetworkType<TestClass> networkType));

            typeHandler.RegisterType<int>((ushort)TypeCode.Int32, () => 0, (num) => { }, intSerializer, intSerializer);

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
            INetworkTypeHandler typeHandler = new DefaultNetworkTypeHandler(pointerProvider);
            INetworkMethodHandler methodHandler = new DefaultMethodHandler(pointerProvider, typeHandler);
            var intSerializer = new IntSerializer();

            INetPtr typePtr = typeHandler.RegisterType<TestClass>(0x01, () => new TestClass());

            Assert.True(typeHandler.TryGetType<TestClass>(out INetworkType<TestClass> networkType));

            typeHandler.RegisterType<int>((ushort)TypeCode.Int32, () => 0, (num) => { }, intSerializer, intSerializer);

            INetPtr methodPtr = methodHandler.Register(typeof(TestClass).GetMethod(nameof(TestClass.Adder)));

            Assert.Equal(1, methodPtr.PtrAddress);

            // create an instance to call the method on
            INetPtr<TestClass> instancePtr = networkType.AllocPtr().As<TestClass>();

            IPacket inputPacket = Packet.Create(0);

            // append the instance ptr to the packet so we know which instance to invoke the method on
            inputPacket.AppendSerializable(instancePtr);

            inputPacket.AppendInt(12);

            inputPacket.AppendInt(12);

            IPacket outputPacket = Packet.Create(0);

            methodHandler.Invoke(methodPtr, inputPacket);

            // get the instance value and make sure RanTest is tru
            TestClass instance = networkType.GetPtr(instancePtr);

            Assert.True(instance.RanAdder);
        }

        [Fact]
        public void Test_Instance_Adder_ReturnValue()
        {
            IPointerProvider pointerProvider = new DefaultPointerProvider();
            INetworkTypeHandler typeHandler = new DefaultNetworkTypeHandler(pointerProvider);
            INetworkMethodHandler methodHandler = new DefaultMethodHandler(pointerProvider, typeHandler);
            var intSerializer = new IntSerializer();

            INetPtr typePtr = typeHandler.RegisterType<TestClass>(0x01, () => new TestClass());

            Assert.True(typeHandler.TryGetType<TestClass>(out INetworkType<TestClass> networkType));

            typeHandler.RegisterType<int>((ushort)TypeCode.Int32, () => 0, (num) => { }, intSerializer, intSerializer);

            INetPtr methodPtr = methodHandler.Register(typeof(TestClass).GetMethod(nameof(TestClass.Adder)));

            Assert.Equal(1, methodPtr.PtrAddress);

            // create an instance to call the method on
            INetPtr<TestClass> instancePtr = networkType.AllocPtr().As<TestClass>();

            IPacket inputPacket = Packet.Create(0);

            // append the instance ptr to the packet so we know which instance to invoke the method on
            inputPacket.AppendSerializable(instancePtr);

            inputPacket.AppendInt(12);

            inputPacket.AppendInt(12);

            IPacket outputPacket = Packet.Create(0);

            methodHandler.Invoke(methodPtr, inputPacket, outputPacket);

            // get the instance value and make sure RanTest is tru
            TestClass instance = networkType.GetPtr(instancePtr);

            Assert.True(instance.RanAdder);

            Assert.Equal(24, outputPacket.GetInt());
        }

        [Fact]
        public void Test_Static_Adder_ReturnValue()
        {
            IPointerProvider pointerProvider = new DefaultPointerProvider();
            INetworkTypeHandler typeHandler = new DefaultNetworkTypeHandler(pointerProvider);
            INetworkMethodHandler methodHandler = new DefaultMethodHandler(pointerProvider, typeHandler);
            var intSerializer = new IntSerializer();

            INetPtr typePtr = typeHandler.RegisterType<TestClass>(0x01, () => new TestClass());

            Assert.True(typeHandler.TryGetType<TestClass>(out INetworkType<TestClass> networkType));

            typeHandler.RegisterType<int>((ushort)TypeCode.Int32, () => 0, (num) => { }, intSerializer, intSerializer);

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
    }
}
