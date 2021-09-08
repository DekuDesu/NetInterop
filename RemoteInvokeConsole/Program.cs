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

namespace RemoteInvokeConsole
{
    class Program
    {
        static int maxClients = 1;
        static readonly Barrier ServerBarrier = new(2);
        static readonly Random Rng = new();
        static readonly int MaxSpeed = 0;
        static long bytesWritten = 0;
        static object writeLock = new();
        static long bytesRead = 0;
        static Stopwatch watch = Stopwatch.StartNew();

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
            Console.ReadLine();

            TokenSource.Cancel();

            Console.WriteLine("Waiting on Tasks");
            await Task.WhenAll(tasks);

            Console.WriteLine("Finished");
        }

        private static void IncrementValue(int value)
        {
            lock (writeLock)
            {
                Volatile.Write(ref bytesWritten, bytesWritten + value);
            }
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

            IPacketDispatcher<PacketType> dispatcher = new PacketDispatcher<PacketType>(stream, headerParser);

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
                    string message = "Hello World!";

                    var packet = Packet.Create(PacketType.String);

                    packet.AppendInt(message.Length);
                    packet.AppendString(message);

                    IncrementValue(packet.Length + sizeof(uint));

                    if (dispatcher.TryWritePacket(packet, out var response, token) && response.PacketType == PacketType.ResponseGood)
                    {
                        Console.WriteLine("Server recognized string packet");
                    }
                    else
                    {
                        throw new InvalidOperationException("Server sent invalid command response");
                    }

                    packet = Packet.Create(PacketType.Int);

                    packet.AppendInt(GetNumber(int.MaxValue));

                    IncrementValue(packet.Length + sizeof(uint));

                    if (dispatcher.TryWritePacket(packet, out response, token) && response.PacketType == PacketType.ResponseGood)
                    {
                        Console.WriteLine("Server recognized int packet");
                    }
                    else
                    {
                        throw new InvalidOperationException("Server sent invalid command response");
                    }

                    packet = Packet.Create(PacketType.Sbyte);

                    packet.AppendSByte(7);

                    IncrementValue(packet.Length + sizeof(uint));

                    if (dispatcher.TryWritePacket(packet, out response, token) && response.PacketType == PacketType.ResponseGood)
                    {
                        Console.WriteLine("Server recognized Sbyte packet");
                    }
                    else
                    {
                        throw new InvalidOperationException("Server sent invalid command response");
                    }

                    packet = Packet.Create(PacketType.Byte);

                    packet.AppendByte(13);

                    IncrementValue(packet.Length + sizeof(uint));

                    if (dispatcher.TryWritePacket(packet, out response, token) && response.PacketType == PacketType.ResponseGood)
                    {
                        Console.WriteLine("Server recognized Byte packet");
                    }
                    else
                    {
                        throw new InvalidOperationException("Server sent invalid command response");
                    }

                    packet = Packet.Create(PacketType.Short);

                    packet.AppendShort(1222);

                    IncrementValue(packet.Length + sizeof(uint));

                    if (dispatcher.TryWritePacket(packet, out response, token) && response.PacketType == PacketType.ResponseGood)
                    {
                        Console.WriteLine("Server recognized Short packet");
                    }
                    else
                    {
                        throw new InvalidOperationException("Server sent invalid command response");
                    }

                    packet = Packet.Create(PacketType.UShort);

                    packet.AppendUShort(5565);

                    IncrementValue(packet.Length + sizeof(uint));

                    if (dispatcher.TryWritePacket(packet, out response, token) && response.PacketType == PacketType.ResponseGood)
                    {
                        Console.WriteLine("Server recognized UShort packet");
                    }
                    else
                    {
                        throw new InvalidOperationException("Server sent invalid command response");
                    }

                    packet = Packet.Create(PacketType.UInt);

                    packet.AppendUInt((uint)GetNumber(int.MaxValue));

                    IncrementValue(packet.Length + sizeof(uint));

                    if (dispatcher.TryWritePacket(packet, out response, token) && response.PacketType == PacketType.ResponseGood)
                    {
                        Console.WriteLine("Server recognized UInt packet");
                    }
                    else
                    {
                        throw new InvalidOperationException("Server sent invalid command response");
                    }

                    packet = Packet.Create(PacketType.Long);

                    packet.AppendLong(long.MaxValue);

                    IncrementValue(packet.Length + sizeof(uint));

                    if (dispatcher.TryWritePacket(packet, out response, token) && response.PacketType == PacketType.ResponseGood)
                    {
                        Console.WriteLine("Server recognized Long packet");
                    }
                    else
                    {
                        throw new InvalidOperationException("Server sent invalid command response");
                    }

                    packet = Packet.Create(PacketType.ULong);

                    packet.AppendLong(long.MaxValue);

                    IncrementValue(packet.Length + sizeof(uint));

                    if (dispatcher.TryWritePacket(packet, out response, token) && response.PacketType == PacketType.ResponseGood)
                    {
                        Console.WriteLine("Server recognized ULong packet");
                    }
                    else
                    {
                        throw new InvalidOperationException("Server sent invalid command response");
                    }

                    packet = Packet.Create(PacketType.Float);

                    packet.AppendFloat(float.MaxValue);

                    IncrementValue(packet.Length + sizeof(uint));

                    if (dispatcher.TryWritePacket(packet, out response, token) && response.PacketType == PacketType.ResponseGood)
                    {
                        Console.WriteLine("Server recognized Float packet");
                    }
                    else
                    {
                        throw new InvalidOperationException("Server sent invalid command response");
                    }

                    packet = Packet.Create(PacketType.Double);

                    packet.AppendDouble(double.MaxValue);

                    IncrementValue(packet.Length + sizeof(uint));

                    if (dispatcher.TryWritePacket(packet, out response, token) && response.PacketType == PacketType.ResponseGood)
                    {
                        Console.WriteLine("Server recognized Double packet");
                    }
                    else
                    {
                        throw new InvalidOperationException("Server sent invalid command response");
                    }

                    packet = Packet.Create(PacketType.Decimal);

                    packet.AppendDecimal(decimal.MaxValue);

                    IncrementValue(packet.Length + sizeof(uint));

                    if (dispatcher.TryWritePacket(packet, out response, token) && response.PacketType == PacketType.ResponseGood)
                    {
                        Console.WriteLine("Server recognized Decimal packet");
                    }
                    else
                    {
                        throw new InvalidOperationException("Server sent invalid command response");
                    }


                    Thread.Sleep(GetNumber(MaxSpeed));

                    //int arraySize = GetNumber(ushort.MaxValue / sizeof(int));
                    int arraySize = ushort.MaxValue / sizeof(int);

                    if (arraySize > 0)
                    {
                        packet = Packet.Create(PacketType.IntArray);

                        packet.Data = new byte[arraySize * sizeof(int)];

                        for (int i = 0; i < arraySize * sizeof(int); i += sizeof(int))
                        {
                            BitConverter.GetBytes(GetNumber(1000)).CopyTo(packet.Data.Slice(i, sizeof(int)));
                        }

                        IncrementValue(packet.Length + sizeof(uint));

                        if (dispatcher.TryWritePacket(packet, out response, token) && response.PacketType == PacketType.ResponseGood)
                        {
                            Console.WriteLine("Server recognized int array packet");
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

            IPacketDispatcher<PacketType> dispatcher = new PacketDispatcher<PacketType>(stream, headerParser);

            try
            {
                while (token.IsCancellationRequested is false)
                {
                    object result = dispatcher.WaitAndConvertPacket<object, PacketType>((ref Packet<PacketType> packet) =>
                     {
                         bytesRead += 4;

                         if (packet.PacketType == PacketType.Int)
                         {
                             bytesRead += 4;
                             return packet.GetInt();
                         }

                         if (packet.PacketType == PacketType.String)
                         {
                             bytesRead += 4;
                             int stringLength = packet.GetInt();

                             bytesRead += stringLength * 2;
                             return packet.GetString(stringLength, Encoding.UTF8);
                         }

                         if (packet.PacketType == PacketType.IntArray)
                         {
                             int size = (int)(packet.Length / sizeof(int));

                             int[] nums = new int[size];

                             for (int i = 0; i < size; i++)
                             {
                                 nums[i] = packet.GetInt();
                                 bytesRead += 4;
                             }

                             return $"int[ {string.Join(", ", nums)} ]";
                         }

                         if (packet.PacketType == PacketType.Sbyte)
                         {
                             bytesRead += sizeof(sbyte);
                             return packet.GetSByte();
                         }

                         if (packet.PacketType == PacketType.Byte)
                         {
                             bytesRead += sizeof(byte);
                             return packet.GetByte();
                         }

                         if (packet.PacketType == PacketType.Short)
                         {
                             bytesRead += sizeof(short);
                             return packet.GetShort();
                         }

                         if (packet.PacketType == PacketType.UShort)
                         {
                             bytesRead += sizeof(ushort);
                             return packet.GetUShort();
                         }

                         if (packet.PacketType == PacketType.UInt)
                         {
                             bytesRead += sizeof(uint);
                             return packet.GetUInt();
                         }

                         if (packet.PacketType == PacketType.Long)
                         {
                             bytesRead += sizeof(long);
                             return packet.GetLong();
                         }

                         if (packet.PacketType == PacketType.ULong)
                         {
                             bytesRead += sizeof(ulong);
                             return packet.GetULong();
                         }

                         if (packet.PacketType == PacketType.Float)
                         {
                             bytesRead += sizeof(float);
                             return packet.GetFloat();
                         }

                         if (packet.PacketType == PacketType.Double)
                         {
                             bytesRead += sizeof(double);
                             return packet.GetDouble();
                         }

                         if (packet.PacketType == PacketType.Decimal)
                         {
                             bytesRead += sizeof(decimal);
                             return packet.GetDecimal();
                         }

                         return null;
                     }, token);

                    if (result == null)
                    {
                        Console.WriteLine($"Worker: Failed to convert recieved data (null)");

                        stream.WriteUInt(headerParser.CreateHeader(0, (byte)PacketType.ResponseBad));
                        IncrementValue(4);
                    }

                    lock (Rng)
                    {
                        Thread.Sleep(Rng.Next(0, MaxSpeed));
                    }


                    Console.WriteLine($"Worker: read value: ({result})");
                    stream.WriteUInt(headerParser.CreateHeader(0, (byte)PacketType.ResponseGood));
                    IncrementValue(4);

                    lock (Rng)
                    {
                        Thread.Sleep(Rng.Next(0, MaxSpeed));
                    }

                    int time = (int)(watch.ElapsedMilliseconds / 1000) + 1;

                    long readSpeed = (bytesRead + 1) / time;

                    long writeSpeed = (bytesWritten + 1) / time;

                    Console.WriteLine($"                                                         Read: {readSpeed / 1000} KB/s Write: {writeSpeed / 1000} KB/s t={watch.ElapsedMilliseconds / 1000} Read: {bytesRead / 1_000_000}MB Written: {bytesWritten / 1_000_000}MB");
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
