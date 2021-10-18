
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

            for (int i = 0; i < 7; i++)
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
                INetPtr<DiagnosticClass> ptr = client.RemoteHeap.Create<DiagnosticClass>().Result;

                if (ptr is null)
                {
                    Console.WriteLine("Failed to get TestClass pointer");
                    continue;
                }

                client.RemoteHeap.Invoke(DiagnosticClass.WritePointer, ptr, "Hello World!");

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
    }
}