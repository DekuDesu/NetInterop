
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

namespace NetInterop.Example
{
    public class Program
    {
        static readonly CancellationTokenSource TokenSource = new();

        static long Count;
        static long Current;

        public static void Main()
        {
            Startup.Initialize();

            var server = Interop.CreateServer();

            server.Start("127.0.0.1", 25565);

            List<Task> tasks = new();

            for (int i = 0; i < 1; i++)
            {
                Console.WriteLine("Starting Task");
                var client = Interop.CreateClient("127.0.0.1", 25565);

                tasks.Add(Task.Run(() => TestRunner(client, TokenSource.Token, Test)));
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

        private static void TestRunner(IClient client, CancellationToken token, Action<IClient, Stopwatch, CancellationToken> Expression)
        {
            Interlocked.CompareExchange(ref loggingProcessorId, Thread.CurrentThread.ManagedThreadId, -1);

            Console.WriteLine("Task Started");
            Stopwatch watch = Stopwatch.StartNew();

            Expression?.Invoke(client, watch, token);

            watch.Stop();
            Console.WriteLine("Task Ended");
        }

        private static void Test(IClient client, Stopwatch watch, CancellationToken token)
        {
            while (token.IsCancellationRequested is false)
            {
                INetPtr<HelloWorld> localPtr = Interop.LocalHeap.Alloc<HelloWorld>();

                HelloWorld local = Interop.LocalHeap.Get(localPtr);

                local.Log();

                INetPtr<HelloWorld> ptr = client.RemoteHeap.Create<HelloWorld>().Result;

                if (ptr is null)
                {
                    Console.WriteLine("Failed to create remote HelloWorld");
                    continue;
                }

                client.RemoteHeap.Invoke(HelloWorld.m_LogMessage, ptr, " World").Wait();

                client.RemoteHeap.Destroy(ptr);
                Interop.LocalHeap.Free(localPtr);

                //Interlocked.Increment(ref Count);

                //Interlocked.Increment(ref Current);

                //if (Thread.CurrentThread.ManagedThreadId == loggingProcessorId)
                //{
                //    if (watch.ElapsedMilliseconds >= 1000)
                //    {
                //        watch.Restart();
                //        Console.WriteLine($"Performed {Current}/{Count} operations in 1 second");
                //        Interlocked.Exchange(ref Current, 0);
                //    }
                //}
            }
        }
    }
}