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
using System.Diagnostics;
using RemoteInvoke.Net.Transport;
using RemoteInvoke.Net.Transport.Extensions;
using RemoteInvoke.Net.Transport.Packets;
using RemoteInvoke.Net.Transport.Packets.Extensions;
using RemoteInvoke.Net.Transport.Abstractions;

namespace RemoteInvokeConsole
{
    class Program
    {
        static int maxClients = 1;
        static readonly Barrier ServerBarrier = new(2);
        static long bytesWritten = 0;
        static object writeLock = new();
        static long bytesRead = 0;
        static object readLock = new();
        static long totalRead = 0;
        static long totalWritten = 0;

        static async Task Main()
        {
            CancellationTokenSource TokenSource = new();

            var host = Dns.GetHostAddresses(Dns.GetHostName());

            var ip = host[0];

            int port = 13000;

            List<Task> tasks = new();

            Task server = Task.Run(() => StartServer(ip, port, TokenSource.Token));

            tasks.Add(server);

            for (int i = 0; i < maxClients; i++)
            {
                Task client = Task.Run(() => StartClient(ip, port, TokenSource.Token));
                tasks.Add(client);
            }

            Console.WriteLine("Press any key to exit");

            //StartDataRateLogger(TokenSource.Token);

            Console.ReadLine();

            TokenSource.Cancel();

            Console.WriteLine("Waiting on Tasks");
            await Task.WhenAll(tasks);

            Console.WriteLine("Finished");
        }

        private static void IncrementWritten(int value)
        {
            lock (writeLock)
            {
                Volatile.Write(ref bytesWritten, bytesWritten + value);
            }
        }
        private static void IncrementRead(int value)
        {
            lock (readLock)
            {
                Volatile.Write(ref bytesRead, bytesRead + value);
            }
        }

        private static void StartDataRateLogger(CancellationToken token)
        {
            int interval = 1000;

            bool stopped = false;

            void WriteDataRates()
            {
                if (stopped) return;

                long read = 0;
                long written = 0;

                lock (readLock)
                {
                    read = bytesRead;
                    bytesRead = 0;
                }

                totalRead += read;

                lock (writeLock)
                {
                    written = bytesWritten;
                    bytesWritten = 0;
                }

                totalWritten += written;

                //Console.WriteLine($"{read}B:{written}B:{totalRead}B:{totalWritten}B");

                Console.WriteLine($"Read: {read} B/s Write: {written} B/s Total RW ({totalRead / 1000}KB : {totalWritten / 1000}KB )");
            }

            System.Timers.Timer timer = new() { AutoReset = true, Enabled = true, Interval = interval };

            timer.Elapsed += (x, y) => WriteDataRates();

            timer.Start();

            token.Register(() =>
            {
                stopped = true;
                timer.Stop();
                Console.WriteLine("Disposed Timer");
                timer.Dispose();
            });
        }

        private static void StartClient(IPAddress address, int port, CancellationToken token)
        {
            //using IClient<NetworkStream> client = new LoggerClientWrapper<NetworkStream>(new DefaultTcpClient(address, port));

            using IClient<NetworkStream> client = new DefaultTcpClient(address, port);

            ServerBarrier.SignalAndWait(token);

            if (client.TryConnect(out _) is false)
            {
                throw new Exception("Client: Failed to connect");
            }

            //IStream<byte> stream = new LoggerStreamWrapper(new ManagedNetworkStream(client)) { MessagePrefix = "Client: " };
            IStream<byte> stream = new ManagedNetworkStream(client);

            IHeaderParser headerParser = new DefaultHeader();

            IPacketController<PacketType> controller = new DefaultPacketController<PacketType>(stream, headerParser);

            var noneHandler = new ActionPacketHandler<PacketType>(PacketType.none, (ref Packet<PacketType> packet) =>
            {
                // we read the header and packet at this point
                IncrementRead(packet.ActualSize);

                //return $"Client:  handled none";
            });

            var intHandler = new ActionPacketHandler<PacketType>(PacketType.ResponseGood, (ref Packet<PacketType> packet) =>
            {
                // we read the header and packet at this point
                IncrementRead(packet.ActualSize);

                string message = "Hello World!";

                packet = new(PacketType.String, Encoding.UTF8.GetByteCount(message) + sizeof(int));

                packet.AppendInt(message.Length);
                packet.AppendString(message, Encoding.UTF8);

                IncrementWritten(packet.ActualSize);

                controller.WriteBlindPacket(packet);

                //return $"Client: received response good from worker";
            });

            var handlers = new IPacketHandler<PacketType>[] { noneHandler, intHandler };


            IPacketDispatcher<PacketType> dispatcher = new DefaultPacketDispatcher<PacketType>(handlers);

            IPacketReceiver<PacketType> receiver = new DefaultPacketReceiver<PacketType>(dispatcher, controller);

            try
            {

                var packet = Packet.Create(PacketType.Int);

                packet.AppendInt(69);

                IncrementWritten(packet.ActualSize);

                controller.WriteBlindPacket(packet);

                receiver.BeginReceiving(token);
            }
            catch (Exception e)
            {
                if (token.IsCancellationRequested)
                {
                    Console.WriteLine("Error caught while token is cancelled:");
                    Console.WriteLine(e.Message);
                    return;
                }
                throw;
            }
        }

        enum PacketType : byte
        {
            none,
            Int,
            String,
            ResponseGood,
            ResponseBad,
            IntArray,
            Sbyte,
            Byte,
            Short,
            UShort,
            UInt,
            Long,
            ULong,
            Float,
            Double,
            Decimal
        }

        private static void Worker(TcpClient client, CancellationToken token)
        {
            Console.WriteLine("Worker: started");
            IStream<byte> stream = new DisposableNetworkStream(client.GetStream());
            //IStream<byte> stream = new LoggerStreamWrapper(new DisposableNetworkStream(client.GetStream())) { MessagePrefix = "Worker: " };

            IHeaderParser headerParser = new DefaultHeader();

            IPacketController<PacketType> controller = new DefaultPacketController<PacketType>(stream, headerParser);

            var noneHandler = new ActionPacketHandler<PacketType>(PacketType.none, (ref Packet<PacketType> packet) =>
            {
                // we read the header and packet at this point
                IncrementRead(packet.ActualSize);

                stream.WriteUInt(headerParser.CreateHeader(0, (byte)PacketType.ResponseGood));

                IncrementWritten(sizeof(uint));

                //return $"Handled: none";
            });

            var intHandler = new ActionPacketHandler<PacketType>(PacketType.Int, (ref Packet<PacketType> packet) =>
            {
                // we read the header and packet at this point
                IncrementRead(packet.ActualSize);

                stream.WriteUInt(headerParser.CreateHeader(0, (byte)PacketType.ResponseGood));

                IncrementWritten(sizeof(uint));

                //return $"Handled: (int){packet.GetInt()}";
            });

            var stringHandler = new ActionPacketHandler<PacketType>(PacketType.String, (ref Packet<PacketType> packet) =>
            {
                // we read the header and packet at this point
                IncrementRead(packet.ActualSize);

                stream.WriteUInt(headerParser.CreateHeader(0, (byte)PacketType.ResponseGood));

                IncrementWritten(sizeof(uint));

                int messageLength = packet.GetInt();

                string message = packet.GetString(messageLength, Encoding.UTF8);

                //return $"Handled: (string){message}";
            });

            var handlers = new IPacketHandler<PacketType>[] { noneHandler, intHandler, stringHandler };

            IPacketDispatcher<PacketType> dispatcher = new DefaultPacketDispatcher<PacketType>(handlers);

            IPacketReceiver<PacketType> receiver = new DefaultPacketReceiver<PacketType>(dispatcher, controller);

            try
            {
                receiver.BeginReceiving(token);
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
