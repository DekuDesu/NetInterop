using NetInterop.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;
using NetInterop.Clients;
using NetInterop.Servers;
using NetInterop.Runtime.MethodHandling;
using NetInterop.Transport.Core.Abstractions.Runtime;
using NetInterop.Transport.Core.Runtime;
using NetInterop.Transport.Core.Abstractions.Packets;
using NetInterop.Transport.Sockets.Server;
using NetInterop.Transport.Core.Packets.Senders;
using NetInterop.Transport.Core.Packets;
using System.Net.Sockets;
using NetInterop.Transport.Core.Packets.Handlers;
using NetInterop.Runtime.Jobs;
using System.Net;
using NetInterop.Transport.Core.Abstractions.Connections;
using NetInterop.Runtime;
using NetInterop.Runtime.TypeHandling;

namespace NetInterop
{
    public static class Interop
    {
        public static IWorkPool WorkPool { get; set; } = new DefaultWorkPool();

        public static ITypeHandler Types { get; set; }
        public static IMethodHandler Methods { get; set; }
        public static IPointerProvider PointerProvider { get; set; }
        public static IObjectHeap LocalHeap { get; set; }

        public static event Action<IClient> OnNewClientConnected;

        static Interop()
        {
            PointerProvider = new DefaultPointerProvider();
            Types = new NetTypeHandler(PointerProvider);
            LocalHeap = new RuntimeHeap(Types,PointerProvider);
            Methods = new DefaultMethodHandler(PointerProvider,Types,LocalHeap);
        }

        public static IClient CreateClient(string hostname, int port)
        {
            var client = new TcpClient();
            var connection = new DefaultTcpConnection(client, IPAddress.Parse(hostname), port);

            return CreateClient(client, connection);
        }

        public static IClient CreateClient(TcpClient client, IConnection connection)
        {
            var BackingClient = client;

            var PacketCallbackHandler = new DefaultPacketDelegateHandler();

            var Connection = connection;

            Connection.Connect();

            var Stream = new DefaultTcpStream(BackingClient, Connection);

            var PacketController = new DefaultPacketController(Stream);

            var PacketSender = new PacketWorkPoolSender(new DefaultPacketSender(PacketController), WorkPool);

            var PointerResponseSender = new DefaultPointerResponseSender(PacketSender);

            var RemoteHeap = new NetworkHeap(Types, PacketSender, PacketCallbackHandler, Methods);

            var PacketHandler = new PacketWorkPoolHandler(WorkPool, new PointerPacketDispatchHandler(
                new IPacketHandler<PointerOperations>[]
                {
                    // pointer operations
                    new AllocPointerHandler(LocalHeap,PointerProvider,PointerResponseSender),
                    new FreePointerHandler(LocalHeap,PointerProvider,PointerResponseSender),
                    new SetPointerHandler(LocalHeap,Types,PointerProvider,PointerResponseSender),
                    new GetPointerHandler(LocalHeap,Types,PointerProvider,PointerResponseSender),
                    new InvokePointerHandler(PointerProvider,PointerResponseSender, Methods),
                    // in charge of handling the results of the above operations
                    new DefaultPointerReponseHandler(new CallbackPacketHandler(PacketCallbackHandler))
                }
            ));

            var PacketReceiver = new DefaultPacketReceiver(PacketHandler, PacketController, Connection);

            WorkPool.AddWork(new PacketReceiverJob(PacketReceiver, PacketController));

            return new InteropClient()
            {
                BackingClient = BackingClient,
                WorkPool = WorkPool,
                Heap = LocalHeap,
                Stream = Stream,
                Methods = Methods,
                Types = Types,
                RemoteHeap = RemoteHeap,
                PacketHandler = PacketHandler,
                PacketReceiver = PacketReceiver,
                PointerResponseSender = PointerResponseSender,
                PacketController = PacketController,
                Connection = Connection,
                PointerProvider = PointerProvider,
                PacketCallbackHandler = PacketCallbackHandler,
                PacketSender = PacketSender
            };
        }

        public static IServer CreateServer()
        {
            var clientHandler = new InteropClientHandler();

            InteropServer result = new InteropServer() { Handler = clientHandler };

            clientHandler.OnHandle += OnNewClientConnected;

            return result;
        }
    }
}
