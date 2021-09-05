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
using System.Diagnostics;

namespace RemoteInvokeConsole
{
    class Program
    {
        static readonly Barrier ServerBarrier = new(2);
        static readonly Random Rng = new();
        static readonly int MaxSpeed = 0;

        static async Task Main()
        {
            CancellationTokenSource TokenSource = new();

            var host = Dns.GetHostAddresses(Dns.GetHostName());

            var ip = host[0];

            int port = 13000;

            Task client = Task.Run(() => StartClient(ip, port, TokenSource.Token));

            Task server = Task.Run(() => StartServer(ip, port, TokenSource.Token));

            Task[] tasks = { client, server };

            Console.WriteLine("Press any key to exit");
            Console.ReadLine();

            TokenSource.Cancel();

            Console.WriteLine("Waiting on Tasks");
            await Task.WhenAll(tasks);

            Console.WriteLine("Finished");
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

            IPacketDispatcher dispatcher = new DefaultPacketDispatcher(stream, headerParser);

            bool VerifyServerResponse()
            {
                byte response = dispatcher.WaitAndConvertPacket<byte>((packet) =>
                {
                    if ((PacketType)packet.PacketType != PacketType.Response)
                    {
                        return 0;
                    }

                    return (byte)packet.Packet.ReadByte();
                }, token);

                return response == 255;
            }

            int bytesSent = 1;

            Stopwatch watch = Stopwatch.StartNew();

            int GetNumber(int upper)
            {
                lock (Rng)
                {
                    return Rng.Next(0, upper);
                }
            }

            try
            {
                while (token.IsCancellationRequested is false)
                {
                    Console.WriteLine($"                                                         {(bytesSent + 1) / (((watch.ElapsedMilliseconds + 1) / 1000) + 1)} B/s");

                    string message = "Hello World!";

                    ushort packetSize = (ushort)(sizeof(int) + message.Length);

                    uint header = headerParser.CreateHeader(packetSize, (byte)PacketType.String);

                    stream.WriteUInt(header);

                    bytesSent += 4;

                    Thread.Sleep(GetNumber(MaxSpeed));

                    stream.WriteInt(message.Length);

                    bytesSent += 4;

                    Thread.Sleep(GetNumber(MaxSpeed));

                    stream.WriteString(message, Encoding.UTF8);

                    bytesSent += 4;

                    if (VerifyServerResponse())
                    {
                        Console.WriteLine("Server recognized string packet");
                    }
                    else
                    {
                        throw new InvalidOperationException("Server sent invalid command response");
                    }

                    header = headerParser.CreateHeader(4, (byte)PacketType.Int);

                    bytesSent += 4;

                    stream.WriteUInt(header);

                    bytesSent += 4;

                    stream.WriteInt(GetNumber(int.MaxValue));

                    Thread.Sleep(GetNumber(MaxSpeed));

                    if (VerifyServerResponse())
                    {
                        Console.WriteLine("Server recognized int packet");
                    }
                    else
                    {
                        throw new InvalidOperationException("Server sent invalid command response");
                    }

                    //int arraySize = GetNumber(ushort.MaxValue / sizeof(int));
                    int arraySize = ushort.MaxValue / sizeof(int);

                    if (arraySize > 0)
                    {
                        packetSize = (ushort)(arraySize * sizeof(int));

                        header = headerParser.CreateHeader(packetSize, (byte)PacketType.IntArray);

                        stream.WriteUInt(header);

                        Thread.Sleep(GetNumber(MaxSpeed));

                        Span<byte> data = new byte[arraySize * sizeof(int)];

                        for (int i = 0; i < arraySize * sizeof(int); i += sizeof(int))
                        {
                            BitConverter.GetBytes(GetNumber(1000)).CopyTo(data.Slice(i, sizeof(int)));
                        }
                        stream.Write(data);

                        bytesSent += packetSize;

                        Thread.Sleep(GetNumber(MaxSpeed));

                        if (VerifyServerResponse())
                        {
                            Console.WriteLine("Server recognized int array");
                        }
                        else
                        {
                            throw new InvalidOperationException("Server sent invalid command response");
                        }
                    }

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
        }

        enum PacketType : byte
        {
            none,
            Int,
            String,
            Response,
            IntArray
        }

        private static void Worker(TcpClient client, CancellationToken token)
        {
            Console.WriteLine("Worker: started");
            IStream<byte> stream = new DisposableNetworkStream(client.GetStream());
            //IStream<byte> stream = new LoggerStreamWrapper(new DisposableNetworkStream(client.GetStream())) { MessagePrefix = "Worker: " };

            IHeaderParser headerParser = new DefaultHeader();

            IPacketDispatcher dispatcher = new DefaultPacketDispatcher(stream, headerParser);

            try
            {
                while (token.IsCancellationRequested is false)
                {
                    object result = dispatcher.WaitAndConvertPacket<object>((packet) =>
                    {
                        if ((PacketType)packet.PacketType == PacketType.Int)
                        {
                            return packet.Packet.ReadInt();
                        }

                        if ((PacketType)packet.PacketType == PacketType.String)
                        {
                            int stringLength = packet.Packet.ReadInt();
                            return packet.Packet.ReadString(stringLength, Encoding.UTF8);
                        }

                        if ((PacketType)packet.PacketType == PacketType.IntArray)
                        {
                            int size = (int)(packet.Packet.Length / sizeof(int));

                            int[] nums = new int[size];

                            for (int i = 0; i < size; i++)
                            {
                                nums[i] = packet.Packet.ReadInt();
                            }

                            return $"int[ {string.Join(", ", nums)} ]";
                        }

                        return null;
                    }, token);

                    if (result == null)
                    {
                        Console.WriteLine($"Worker: Failed to convert recieved data (null)");

                        stream.WriteUInt(headerParser.CreateHeader(1, (byte)PacketType.Response));
                        stream.WriteByte(0);
                    }

                    lock (Rng)
                    {
                        Thread.Sleep(Rng.Next(0, MaxSpeed));
                    }


                    Console.WriteLine($"Worker: read value: ({result})");
                    stream.WriteUInt(headerParser.CreateHeader(1, (byte)PacketType.Response));
                    stream.WriteByte(255);

                    lock (Rng)
                    {
                        Thread.Sleep(Rng.Next(0, MaxSpeed));
                    }
                }
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
