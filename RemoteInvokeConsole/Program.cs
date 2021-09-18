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
using NetInterop.Transport.Core.Abstractions.Packets;
using NetInterop.Transport.Core.Packets;
using NetInterop.Transport.Core;
using NetInterop.Transport.Core.Packets.Extensions;
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
        static object rngLock = new();
        static Random randomGenerator = new();

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

        private static int GetRandomNumber(int lower = int.MinValue, int upper = int.MaxValue)
        {
            lock (rngLock)
            {
                return randomGenerator.Next(lower, upper);
            }
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
            TcpClient client = new();

            IConnection connection = new DefaultTcpConnection(client, address, port);

            ServerBarrier.SignalAndWait(token);

            connection.Connect();

            IStream<byte> stream = new DefaultTcpStream(client, connection);

            IPacketHeader<PacketType> headerParser = new DefaultHeader<PacketType>(num => (PacketType)num);

            IPacketController<PacketType> controller = new DefaultPacketController<PacketType>(stream, headerParser);

            IPacketSender<PacketType> sender = new PacketSenderSizeLogger(new DefaultPacketSender<PacketType>(controller));

            int[] randomNumbers = new int[(ushort.MaxValue / sizeof(int)) - 10];

            for (int i = 0; i < randomNumbers.Length; i++)
            {
                randomNumbers[i] = GetRandomNumber();
            }

            var handlers = new IPacketHandler<PacketType>[] { new GoodResponseHandler(), new ConnectionAliveHandler(), new IntHandler("Client") };

            IPacketDispatcher<PacketType> dispatcher = new PacketDispatcherSizeLogger(new DefaultPacketDispatcher<PacketType>(handlers));

            IPacketReceiver<PacketType> receiver = new DefaultPacketReceiver<PacketType>(dispatcher, controller, connection);

            var intArrayPacket = new IntArrayPacket(randomNumbers);

            try
            {
                receiver.BeginReceiving();

                while (token.IsCancellationRequested is false)
                {
                    sender.Send(new Message("Hello World!"));
                    Thread.Sleep(1);
                    sender.Send(intArrayPacket);
                }
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
            finally
            {
                receiver.StopReceiving();
            }
        }

        public class IntArrayPacket : IPacketSerializable<PacketType>
        {
            private readonly int[] data;

            public IntArrayPacket(int[] data)
            {
                this.data = data;
            }

            public PacketType PacketType { get; } = PacketType.IntArray;

            public int EstimatePacketSize() => data.Length * sizeof(int);

            public void Serialize(IPacket<PacketType> packetBuilder)
            {
                packetBuilder.AppendArray(data);
            }
        }

        public class IntArrayHandler : IPacketHandler<PacketType>
        {
            public PacketType PacketType { get; } = PacketType.IntArray;

            public void Handle(IPacket<PacketType> packet)
            {
                int[] arr = packet.GetIntArray();

                _ = arr[0];
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

            public void Serialize(IPacket<PacketType> packetBuilder)
            {
                packetBuilder.AppendString(Value, Encoding.UTF8);
            }
        }

        public class StringHandler : IPacketHandler<PacketType>
        {
            private readonly string MessagePrefix;

            public StringHandler(string messagePrefix)
            {
                MessagePrefix = messagePrefix ?? throw new ArgumentNullException(nameof(messagePrefix));
            }

            public PacketType PacketType { get; } = PacketType.String;

            public void Handle(IPacket<PacketType> packet)
            {
                _ = packet.GetString(Encoding.UTF8);
                //Console.WriteLine($"{MessagePrefix}: {packet.GetString(Encoding.UTF8)}");
            }
        }

        public class PacketDispatcherSizeLogger : IPacketDispatcher<PacketType>
        {
            private readonly IPacketDispatcher<PacketType> dispatcher;

            public PacketDispatcherSizeLogger(IPacketDispatcher<PacketType> dispatcher)
            {
                this.dispatcher = dispatcher ?? throw new ArgumentNullException(nameof(dispatcher));
            }

            public void Dispatch(IPacket<PacketType> packet)
            {
                IncrementRead(packet.ActualSize);
                dispatcher.Dispatch(packet);
            }
        }

        public class PacketSenderSizeLogger : IPacketSender<PacketType>
        {
            private readonly IPacketSender<PacketType> sender;

            public PacketSenderSizeLogger(IPacketSender<PacketType> sender)
            {
                this.sender = sender;
            }

            public void Send(IPacketSerializable<PacketType> value)
            {
                IncrementWritten(value.EstimatePacketSize() + sizeof(uint));
                sender.Send(value);
            }

            public void Send(IPacket<PacketType> packet)
            {
                IncrementWritten(packet.ActualSize);
                sender.Send(packet);
            }

            public void Send(PacketType packetType, byte[] data)
            {
                IncrementWritten(sizeof(uint) + data.Length);
                sender.Send(packetType, data);
            }
        }

        public class IntHandler : IPacketHandler<PacketType>
        {
            private readonly string MessagePrefix;

            public IntHandler(string messagePrefix)
            {
                MessagePrefix = messagePrefix ?? throw new ArgumentNullException(nameof(messagePrefix));
            }

            public PacketType PacketType { get; } = PacketType.Int;

            public void Handle(IPacket<PacketType> packet)
            {
                _ = packet.GetInt();
                //Console.WriteLine($"{MessagePrefix}: {packet.GetInt()}");
            }
        }

        public class IntPacket : IPacketSerializable<PacketType>
        {
            public int Value { get; private set; }
            public PacketType PacketType { get; } = PacketType.Int;

            public int EstimatePacketSize() => sizeof(int);


            public void NewNumber()
            {
                Value = GetRandomNumber();
            }

            public void Serialize(IPacket<PacketType> packetBuilder)
            {
                packetBuilder.AppendInt(Value);
            }
        }

        public class GoodResponseHandler : IPacketHandler<PacketType>
        {
            public PacketType PacketType { get; } = PacketType.ResponseGood;

            public void Handle(IPacket<PacketType> packet) { }
        }

        public class ConnectionAliveHandler : IPacketHandler<PacketType>
        {
            public PacketType PacketType { get; } = PacketType.none;

            public void Handle(IPacket<PacketType> packet) { }
        }

        public class ConnectionAlivePacket : IPacketSerializable<PacketType>
        {
            public PacketType PacketType { get; } = PacketType.none;

            public int EstimatePacketSize() => 0;

            public void Serialize(IPacket<PacketType> packetBuilder) { }
        }

        public enum PacketType : ushort
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

        private static void Worker(IConnection connection, TcpClient client, CancellationToken token)
        {
            Console.WriteLine("Worker: started");
            IStream<byte> stream = new DefaultTcpStream(client, connection);

            IPacketHeader<PacketType> headerParser = new DefaultHeader<PacketType>((num) => (PacketType)num);

            IPacketController<PacketType> controller = new DefaultPacketController<PacketType>(stream, headerParser);

            var connectionAliveHandler = new ConnectionAliveHandler();

            var handlers = new IPacketHandler<PacketType>[] { new StringHandler("Server"), connectionAliveHandler, new IntHandler("Server"), new IntArrayHandler() };

            IPacketDispatcher<PacketType> dispatcher = new PacketDispatcherSizeLogger(new DefaultPacketDispatcher<PacketType>(handlers));

            IPacketReceiver<PacketType> receiver = new DefaultPacketReceiver<PacketType>(dispatcher, controller, connection);

            IPacketSender<PacketType> sender = new PacketSenderSizeLogger(new DefaultPacketSender<PacketType>(controller));

            var packet = new IntPacket();

            try
            {
                receiver.BeginReceiving();

                while (token.IsCancellationRequested is false)
                {
                    packet.NewNumber();
                    //Console.WriteLine($"Worker: {packet.Value}");
                    sender.Send(packet);
                    Thread.Sleep(1);
                }
            }
            finally
            {
                receiver.StopReceiving();
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
                connection.Disconnected += connection => Console.WriteLine("Server Connection: disconnected");
                workers.Add(Task.Run(() => Worker(connection, newClient, token), token));
            });

            IClientDispatcher<TcpClient> dispatcher = new DefaultClientDispatcher<TcpClient>(handler);

            IConnectionManager connectionManager = new DefaultTcpListenerConnectionManager(new TcpListener(address, port), dispatcher, new DefaultClientProvider<TcpClient>((client) => new DefaultTcpServerClientConnection(client)));

            connectionManager.Connected += connection => Console.WriteLine("Connection Manager: connected Client");

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
