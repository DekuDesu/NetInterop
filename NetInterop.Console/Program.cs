
using System;
using NetInterop;
using NetInterop.Abstractions;
using NetInterop.Transport.Core.Abstractions.Packets;
using NetInterop.Transport.Core.Packets.Extensions;
using NetInterop.Runtime.Extensions;
using NetInterop.Transport.Core.Runtime;

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



            Console.ReadLine();

            server.Stop();
        }

        private static void Startup(INetTypeHandler handler)
        {
            var serializer = new TestClassSerializer();
            handler.RegisterType<TestClass>(0x01,serializer,serializer,serializer,serializer);
        }
        public static void Startup(INetworkMethodHandler handler)
        {
            handler.Register<TestClass>((a)=>nameof(a.SetValue));
            handler.Register<TestClass>((a) => nameof(a.GetValue));
        }
        public class TestClass :IDisposable
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
    }
}