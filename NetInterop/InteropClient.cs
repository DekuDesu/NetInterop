using NetInterop.Abstractions;
using NetInterop.Runtime.Jobs;
using NetInterop.Runtime.MethodHandling;
using NetInterop.Transport.Core.Abstractions;
using NetInterop.Transport.Core.Abstractions.Connections;
using NetInterop.Transport.Core.Abstractions.Packets;
using NetInterop.Transport.Core.Abstractions.Runtime;
using NetInterop.Transport.Core.Packets;
using NetInterop.Transport.Core.Packets.Handlers;
using NetInterop.Transport.Core.Packets.Senders;
using NetInterop.Transport.Core.Runtime;
using NetInterop.Transport.Sockets.Server;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace NetInterop.Clients
{
    public class InteropClient : IClient
    {
        private TcpClient client;

        public IConnection Connection { get; set; }
        public IStream<byte> Stream { get; set; }
        public INetworkTypeHandler Types { get; set; }
        public INetworkMethodHandler Methods { get; set; }
        public INetworkHeap RemoteHeap { get; set; }
        public IPacketController PacketController { get; set; }
        public IPacketReceiver PacketReceiver { get; set; }
        public IPacketSender PacketSender { get; set; }
        public IPacketHandler PacketHandler { get; set; }

        public IWorkPool WorkPool { get; set; }

        public IPointerProvider PointerProvider { get; set; } = new DefaultPointerProvider();
        public IPointerResponseSender PointerResponseSender { get; set; }
        public IDelegateHandler<bool, IPacket> PacketCallbackHandler { get; set; }


        public void Connect(string hostname, int port)
        {
            Connection = new DefaultTcpConnection(client, IPAddress.Parse(hostname), port);

            Stream = new DefaultTcpStream(client, Connection);

            PacketController = new DefaultPacketController(Stream);

            WorkPool = new DefaultWorkPool();

            PacketSender = new PacketWorkPoolSender(new DefaultPacketSender(PacketController), WorkPool);

            Types = new DefaultNetworkTypeHandler(PointerProvider);

            Methods = new DefaultMethodHandler(PointerProvider, Types);

            PointerResponseSender = new DefaultPointerResponseSender(PacketSender);

            PacketCallbackHandler = new DefaultPacketDelegateHandler();

            RemoteHeap = new NetworkHeap(Types, PacketSender, PacketCallbackHandler, Methods);

            PacketHandler = new PacketWorkPoolHandler(WorkPool, new PointerPacketDispatchHandler(
                new IPacketHandler<PointerOperations>[]
                {
                    // pointer operations
                    new AllocPointerHandler(Types,PointerProvider,PointerResponseSender),
                    new FreePointerHandler(Types,PointerProvider,PointerResponseSender),
                    new SetPointerHandler(Types,PointerProvider,PointerResponseSender),
                    new GetPointerHandler(Types,PointerProvider,PointerResponseSender),
                    new InvokePointerHandler(Types,PointerProvider,PointerResponseSender, Methods),
                    // in charge of handling the results of the above operations
                    new DefaultPointerReponseHandler(new CallbackPacketHandler(PacketCallbackHandler))
                }
            ));

            PacketReceiver = new DefaultPacketReceiver(PacketHandler, PacketController, Connection);

            WorkPool.AddWork(new PacketReceiverJob(PacketReceiver, PacketController));

            WorkPool.StartPool();
        }

        public void Disconnect()
        {
            WorkPool.StopPool();

            PacketReceiver = null;

            PacketHandler = null;

            RemoteHeap = null;

            PacketCallbackHandler = null;

            PointerResponseSender = null;

            Methods.Clear();

            Methods = null;

            Types.Clear();

            Types = null;

            PacketSender = null;

            WorkPool = new DefaultWorkPool();

            PacketController = null;

            Stream = null;

            Connection.Disconnect();

            Connection = null;

            client.Dispose();

            client = new TcpClient();
        }
    }
}
