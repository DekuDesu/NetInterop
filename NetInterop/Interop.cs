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
using NetInterop.Transport.Core.Abstractions;

namespace NetInterop
{
    public static class Interop
    {
        public static bool IsServer { get; private set; }
        public static bool IsClient => !IsServer;
        public static IClient Client { get; set; }

        public static IWorkPool WorkPool { get; set; } = new DefaultWorkPool();
        public static ITypeHandler Types { get; set; }
        public static IMethodHandler Methods { get; set; }
        public static IPointerProvider PointerProvider { get; set; }
        public static IObjectHeap LocalHeap { get; set; }

        /// <summary>
        /// Allows users to override all objects instantiated by this composition class
        /// </summary>
        public static ModifierGroup Modifiers { get; set; } = new ModifierGroup();

        public static event Action<IClient> OnNewClientConnected;

        /// <summary>
        /// Initializes this object's variables to initial use. Modifiers should have any overrides assigned before this is invoked.
        /// </summary>
        public static void Initialize()
        {
            PointerProvider = Modifiers.PointerProvider.Inject(new DefaultPointerProvider());
            Types = Modifiers.TypeHandler.Inject(new NetTypeHandler(PointerProvider));
            LocalHeap = Modifiers.ObjectHeap.Inject(new RuntimeHeap(Types, PointerProvider));
            Methods = Modifiers.MethodHandler.Inject(new DefaultMethodHandler(PointerProvider, Types, LocalHeap));
        }

        public static IClient CreateClient(string hostname, int port)
        {
            var client = new TcpClient();
            var connection = Modifiers.Connection.Inject(new DefaultTcpConnection(client, IPAddress.Parse(hostname), port));

            return CreateClient(client, connection);
        }

        public static IClient CreateClient(TcpClient client, IConnection connection)
        {
            var BackingClient = client;

            var PacketCallbackHandler = Modifiers.CallbackHandler.Inject(new DefaultPacketDelegateHandler());

            var Connection = connection;

            Connection.Connect();

            var Stream = Modifiers.Stream.Inject(new DefaultTcpStream(BackingClient, Connection));

            var PacketController = Modifiers.PacketController.Inject(new DefaultPacketController(Stream));

            var PacketSender = Modifiers.PacketSender.Inject(new PacketWorkPoolSender(new DefaultPacketSender(PacketController), WorkPool));

            var PointerResponseSender = Modifiers.PointerRepsonseSender.Inject(new DefaultPointerResponseSender(PacketSender));

            var RemoteHeap = Modifiers.RemoteHeap.Inject(new RemoteHeap(PointerProvider, PacketSender, Types, Methods, PacketCallbackHandler));

            var PacketHandler = Modifiers.PacketHandler.Inject(new PacketWorkPoolHandler(WorkPool, new PointerPacketDispatchHandler(
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
            )));

            var PacketReceiver = Modifiers.PacketReceiver.Inject(new DefaultPacketReceiver(PacketHandler, PacketController, Connection));

            WorkPool.AddWork(new PacketReceiverJob(PacketReceiver, PacketController));

            return Modifiers.Client.Inject(new InteropClient()
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
            });
        }

        public static IServer CreateServer()
        {
            var interopHandler = new InteropClientHandler();

            interopHandler.OnHandle += OnNewClientConnected;

            var clientHandler = Modifiers.ClientHandler.Inject(interopHandler);

            IServer result = Modifiers.Server.Inject(new InteropServer() { Handler = clientHandler });

            IsServer = true;

            return result;
        }
    }
}
