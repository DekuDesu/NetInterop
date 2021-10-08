
using System;
using NetInterop;
using NetInterop.Abstractions;
using NetInterop.Transport.Core.Abstractions.Packets;
using NetInterop.Transport.Core.Packets.Extensions;
using NetInterop.Runtime.Extensions;
using NetInterop.Transport.Core.Runtime;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Text;
using System.Collections.Generic;

namespace RemoteInvokeConsole
{
    public class Program
    {
        static CancellationTokenSource TokenSource = new();
        static long Count;
        static long Current;
        public static void Main()
        {
            Startup(Interop.Types);
            Startup(Interop.Methods);

            var server = Interop.CreateServer();

            server.Start("127.0.0.1", 25565);

            List<Task> tasks = new List<Task>();

            for (int i = 0; i < (Environment.ProcessorCount / 2) - 2; i++)
            {
                Console.WriteLine("Starting Task");
                var client = Interop.CreateClient("127.0.0.1", 25565);

                tasks.Add(Task.Run(() => Test(server, client, TokenSource.Token)));
            }

            try
            {
                Console.ReadLine();
                Console.WriteLine("Cancelling");
                TokenSource.Cancel();

                Task.WaitAll(tasks.ToArray());

                Console.WriteLine("Cancelled all tasks");
            }
            finally
            {
                server.Stop();
            }
        }
        static int loggingProcessorId = -1;
        private static void Test(IServer server, IClient client, CancellationToken token)
        {
            _ = server;

            Interlocked.CompareExchange(ref loggingProcessorId, Thread.CurrentThread.ManagedThreadId, -1);

            Console.WriteLine("Task Started");
            Stopwatch watch = Stopwatch.StartNew();
            while (token.IsCancellationRequested is false)
            {
                INetPtr<TestClass> ptr = client.RemoteHeap.Create<TestClass>().Result;

                if (ptr is null)
                {
                    Console.WriteLine("Failed to get TestClass pointer");
                    continue;
                }

                client.RemoteHeap.Invoke(WriteMessagePtr, ptr, "Hello World!");

                client.RemoteHeap.Destroy(ptr);

                Interlocked.Increment(ref Count);
                Interlocked.Increment(ref Current);

                if (Thread.CurrentThread.ManagedThreadId == loggingProcessorId)
                {
                    if (watch.ElapsedMilliseconds >= 1000)
                    {
                        watch.Restart();
                        Console.WriteLine($"Performed {Current}/{Count} operations in 1 second");
                        Interlocked.Exchange(ref Current, 0);
                    }
                }
            }
            watch.Stop();
            Console.WriteLine("Task Ended");
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
                _ = message;
                //Console.WriteLine($"TestClass: {message}");
            }

            public void Dispose()
            {
                //Console.WriteLine("Disposed TestClass Instance");
            }
        }

        public class TestClassSerializer :
            IActivator<TestClass>,
            IDeactivator<TestClass>,
            IPacketSerializer<TestClass>,
            IPacketDeserializer<TestClass>
        {
            public object AmbiguousDeserialize(IPacket packet) => Deserialize(packet);

            public void AmbiguousSerialize(object value, IPacket packetBuilder) => Serialize((TestClass)value, packetBuilder);

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

            public int EstimatePacketSize(TestClass value) => sizeof(int);

            public void Serialize(TestClass value, IPacket packetBuilder)
            {
                packetBuilder.AppendInt(value.Value);
            }
        }

        public class IntSerializer : IPacketSerializer<int>, IPacketDeserializer<int>
        {
            public object AmbiguousDeserialize(IPacket packet) => packet.GetInt();

            public void AmbiguousSerialize(object value, IPacket packetBuilder) => Serialize((int)value, packetBuilder);

            public int Deserialize(IPacket packet) => packet.GetInt();

            public int EstimatePacketSize(int value) => sizeof(int);

            public void Serialize(int value, IPacket packetBuilder) => packetBuilder.AppendInt(value);
        }

        public class UTF8Serializer : IPacketSerializer<string>, IPacketDeserializer<string>
        {
            public object AmbiguousDeserialize(IPacket packet) => packet.GetString(System.Text.Encoding.UTF8);

            public void AmbiguousSerialize(object value, IPacket packetBuilder) => Serialize((string)value, packetBuilder);

            public string Deserialize(IPacket packet) => packet.GetString(System.Text.Encoding.UTF8);

            public int EstimatePacketSize(string value) => Encoding.UTF8.GetByteCount(value) + sizeof(int);

            public void Serialize(string value, IPacket packetBuilder) => packetBuilder.AppendString(value, System.Text.Encoding.UTF8);
        }
    }
}