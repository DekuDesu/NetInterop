
using System;
using NetInterop;
using NetInterop.Abstractions;
using NetInterop.Transport.Core.Abstractions.Packets;
using NetInterop.Transport.Core.Packets.Extensions;
using NetInterop.Runtime.Extensions;
using NetInterop.Transport.Core.Runtime;
using System.Threading;

namespace RemoteInvokeConsole
{
    public class Program
    {

        public static void Main()
        {
            Startup(Interop.Types);
            Startup(Interop.Methods);

            var server = Interop.CreateServer();

            server.Start("127.0.0.1", 25565);

            var client = Interop.CreateClient("127.0.0.1", 25565);

            try
            {
                Test(server, client);
                Console.ReadLine();
            }
            finally
            {
                client.Disconnect();
                server.Stop();
            }
        }

        private static void Test(IServer server, IClient client)
        {
            _ = server;

            INetPtr<TestClass> ptr = client.RemoteHeap.Create<TestClass>().Result;

            Console.WriteLine($"Created remote TestClass instance ptr: {ptr}");

            for (int i = 0; i < 10; i++)
            {
                client.RemoteHeap.Invoke(WriteMessagePtr, ptr, "Hello World!");
                Thread.Sleep(100);
            }
        }

        private static void Startup(ITypeHandler handler)
        {
            var serializer = new TestClassSerializer();
            var intSer = new IntSerializer();
            var utf8Serializer = new UTF8Serializer();
            handler.RegisterType<int>((ushort)TypeCode.Int32, intSer, intSer);
            handler.RegisterType<string>((ushort)TypeCode.String, utf8Serializer, utf8Serializer);
            handler.RegisterType<TestClass>(0x01, serializer, serializer, serializer, serializer);
        }

        private static INetPtr SetValuePtr;
        private static INetPtr<int> GetValuePtr;
        private static INetPtr WriteMessagePtr;

        public static void Startup(IMethodHandler handler)
        {
            SetValuePtr = handler.Register<TestClass, int>(a => a.SetValue);
            GetValuePtr = handler.Register<TestClass, int>(a => a.GetValue);
            WriteMessagePtr = handler.Register<TestClass, string>(a => a.Write);
        }

        public class TestClass : IDisposable
        {
            public int Value { get; set; }
            public void SetValue(int value)
            {
                Value = value;
            }
            public int GetValue()
            {
                return Value;
            }
            public void Write(string message)
            {
                Console.WriteLine($"TestClass: {message}");
            }

            public void Dispose()
            {
                Console.WriteLine("Disposed TestClass Instance");
            }
        }

        public class TestClassSerializer :
            IActivator<TestClass>,
            IDeactivator<TestClass>,
            IPacketSerializer<TestClass>,
            IPacketDeserializer<TestClass>
        {
            public object AmbiguousDeserialize(IPacket packet) => Deserialize(packet);

            public TestClass CreateInstance()
            {
                return new TestClass();
            }

            public TestClass Deserialize(IPacket packet)
            {
                return new TestClass() { Value = packet.GetInt() };
            }

            public void DestroyInstance(ref TestClass instance)
            {
                instance?.Dispose();
            }

            public void DestroyInstance(ref object instance)
            {
                if (instance is TestClass isTestClass)
                {
                    isTestClass?.Dispose();
                }
            }

            public void Serialize(TestClass value, IPacket packetBuilder)
            {
                packetBuilder.AppendInt(value.Value);
            }
        }

        public class IntSerializer : IPacketSerializer<int>, IPacketDeserializer<int>
        {
            public object AmbiguousDeserialize(IPacket packet) => packet.GetInt();

            public int Deserialize(IPacket packet) => packet.GetInt();

            public void Serialize(int value, IPacket packetBuilder) => packetBuilder.AppendInt(value);
        }

        public class UTF8Serializer : IPacketSerializer<string>, IPacketDeserializer<string>
        {
            public object AmbiguousDeserialize(IPacket packet) => packet.GetString(System.Text.Encoding.UTF8);

            public string Deserialize(IPacket packet) => packet.GetString(System.Text.Encoding.UTF8);

            public void Serialize(string value, IPacket packetBuilder) => packetBuilder.AppendString(value, System.Text.Encoding.UTF8);
        }
    }
}