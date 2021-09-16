using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Reflection;
using System.IO;
using System.Diagnostics;
using NetInterop.Transport.Core.Abstractions;
using NetInterop.Transport.Servers;
using NetInterop.Transport.Providers;
using NetInterop.Transport.Core.Abstractions.Packets;
using NetInterop.Transport.Core.Packets;
using NetInterop.Transport.Streams;
using NetInterop.Transport.Core;
using RNetInterop.Transport.Core.Packets;
using NetInterop.Transport.Clients;
using NetInterop.Transport.Core.Packets.Extensions;
using NetInterop.Transport.Streams.Extensions;
using NetInterop.Transport.Core.Abstractions.Server;
using NetInterop.Transport.Sockets.Server;

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
        static Random rng = new();

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

            StartDataRateLogger(TokenSource.Token);

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
            string speedUnit = "MB";
            int speedModifier = 1000000;

            string totalUnit = "MB";
            int totalModifier = 1000000;

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

                Console.WriteLine($"Read: {read / speedModifier} {speedUnit}/s Write: {written / speedModifier} {speedUnit}/s Total RW ({totalRead / totalModifier}{totalUnit} : {totalWritten / totalModifier}{totalUnit} )");
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

            IPacketSender<PacketType> sender = new DefaultPacketSender<PacketType>(controller);

            var noneHandler = new ActionPacketHandler<PacketType>(PacketType.none, (ref Packet<PacketType> packet) =>
            {
                // we read the header and packet at this point
                IncrementRead(packet.ActualSize);

                //return $"Client:  handled none";
            });

            int[] randomNumbers = new int[(ushort.MaxValue / sizeof(int)) - 10];

            for (int i = 0; i < randomNumbers.Length; i++)
            {
                randomNumbers[i] = rng.Next();
            }

            var intHandler = new ActionPacketHandler<PacketType>(PacketType.ResponseGood, (ref Packet<PacketType> packet) =>
            {
                // we read the header and packet at this point
                IncrementRead(packet.ActualSize);

                sender.Send(new Message("Hello World!"));

                IncrementWritten(packet.ActualSize);

                packet = Packet.Create(PacketType.IntArray);

                packet.AppendArray(randomNumbers);

                sender.Send(packet);

                IncrementWritten(packet.ActualSize);

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

        public class Message : IPacketSerializable<PacketType>
        {
            public string Value { get; set; }

            public Message(string value)
            {
                Value = value;
            }

            public PacketType PacketType { get; } = PacketType.String;

            public int EstimatePacketSize() => Encoding.UTF8.GetByteCount(Value);

            public void Serialize(ref Packet<PacketType> packetBuilder)
            {
                packetBuilder.AppendString(Value, Encoding.UTF8);
            }
        }

        public enum PacketType : byte
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

            var intArrayHandler = new ActionPacketHandler<PacketType>(PacketType.IntArray, (ref Packet<PacketType> packet) =>
            {
                // we read the header and packet at this point
                IncrementRead(packet.ActualSize);

                stream.WriteUInt(headerParser.CreateHeader(0, (byte)PacketType.ResponseGood));

                IncrementWritten(sizeof(uint));

                int[] data = packet.GetIntArray();

                _ = data[0];
                //return $"Handled: (int[]){data.Length} First number: {data[0]}";
            });

            var stringHandler = new ActionPacketHandler<PacketType>(PacketType.String, (ref Packet<PacketType> packet) =>
            {
                // we read the header and packet at this point
                IncrementRead(packet.ActualSize);

                stream.WriteUInt(headerParser.CreateHeader(0, (byte)PacketType.ResponseGood));

                IncrementWritten(sizeof(uint));

                string message = packet.GetString(Encoding.UTF8);

                //return $"Handled: (string){message}";
            });

            var handlers = new IPacketHandler<PacketType>[] { noneHandler, intHandler, stringHandler, intArrayHandler };

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
            List<Task> workers = new();

            IClientHandler<TcpClient> handler = new DefaultClientHandler<TcpClient>((newClient, connection) =>
            {
                workers.Add(Task.Run(() => Worker(newClient, token), token));
            });

            IClientDispatcher<TcpClient> dispatcher = new DefaultClientDispatcher<TcpClient>(handler);

            IConnectionManager connectionManager = new DefaultTcpListenerConnectionManager(new TcpListener(address, port), dispatcher);

            try
            {
                connectionManager.StartConnecting();

                ServerBarrier.SignalAndWait(token);

                while (token.IsCancellationRequested is false)
                {
                    Thread.Sleep(10);
                }
            }
            finally
            {
                connectionManager.DisconnectAll();
            }

            Console.WriteLine("Server: waiting for workers");
            Task.WaitAll(workers.ToArray(), CancellationToken.None);
            Console.WriteLine($"Sever: all workers ended");
        }
    }
}
