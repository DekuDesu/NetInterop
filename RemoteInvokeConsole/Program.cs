using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using RemoteInvoke.Net;
using RemoteInvoke.Net.Server;
using RemoteInvoke.Net.Client;
using System.Reflection;
using System.IO;
using RemoteInvoke.Runtime.Data;
using RemoteInvoke.Runtime.Data.Helpers;

namespace RemoteInvokeConsole
{
    class Program
    {
        static readonly Barrier ServerBarrier = new(2);
        static readonly Random Rng = new();
        static async Task Main()
        {
            CancellationTokenSource TokenSource = new();

            var host = Dns.GetHostAddresses(Dns.GetHostName());

            var ip = host[0];

            int port = 13000;

            Task[] tasks = { Task.Run(() => StartServer(ip, port, TokenSource.Token)), Task.Run(() => StartClient(ip, port, TokenSource.Token)) };

            Console.WriteLine("Press any key to exit");
            Console.ReadLine();

            TokenSource.Cancel();

            Console.WriteLine("Waiting on Tasks");
            await Task.WhenAll(tasks);

            Console.WriteLine("Finished");
        }

        private static byte[] LoadAssembly()
        {
            // this loads a testing assembly to test
            /*
                static int Main(params string[] args)
                {
                    Console.WriteLine($"HelloWorld!{string.Join("",args)}");
                    return 1;
                }
            */
            return File.ReadAllBytes("HelloWorld.dll");
        }

        private static void StartClient(IPAddress address, int port, CancellationToken token)
        {
            using IClient<NetworkStream> client = new LoggerClientWrapper<NetworkStream>(new DefaultTcpClient(address, port));

            ServerBarrier.SignalAndWait(token);

            if (client.TryConnect(out _) is false)
            {
                throw new Exception("Client: Failed to connect");
            }

            IStream<byte> stream = new LoggerStreamWrapper(new ManagedNetworkStream(client)) { MessagePrefix = "Client: " };

            stream.WriteInt(4);
        }

        private static void Worker(TcpClient client, CancellationToken token)
        {
            Console.WriteLine("Worker: started");
            IStream<byte> stream = new LoggerStreamWrapper(new DisposableNetworkStream(client.GetStream())) { MessagePrefix = "Worker: " };

            IPayloadDispatcher dispatcher = new PayloadDispatcher(stream);

            try
            {

            }
            finally
            {
                client.Close();
                client.Dispose();
                Console.WriteLine("Worker: stopped");
            }
        }

        private static void StartServer(IPAddress address, int port, CancellationToken token)
        {

            IServer<TcpClient> server = new LoggerServerWrapper<TcpClient>(new DefaultServer<TcpClient>(port, new TcpClientProvider(address, port)));

            List<Task> workers = new();

            server.Start();

            ServerBarrier.SignalAndWait(token);

            try
            {
                server.BeginAcceptingClients(newClient =>
                {
                    workers.Add(Task.Run(() => Worker(newClient, token), token));
                }, token);
            }
            finally
            {
                server.CloseConnections();
                server.Stop();
            }

            Console.WriteLine("Waiting for workers to end");
            Task.WaitAll(workers.ToArray(), CancellationToken.None);
        }
    }
}
