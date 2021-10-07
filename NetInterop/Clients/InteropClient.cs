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
        public TcpClient BackingClient { get; set; }

        public IConnection Connection { get; set; }
        public IStream<byte> Stream { get; set; }
        public ITypeHander Types { get; set; }
        public IMethodHandler Methods { get; set; }
        public INetworkHeap RemoteHeap { get; set; }
        public IPacketController PacketController { get; set; }
        public IPacketReceiver PacketReceiver { get; set; }
        public IPacketSender PacketSender { get; set; }
        public IPacketHandler PacketHandler { get; set; }
        public IObjectHeap Heap { get; set; }
        public IWorkPool WorkPool { get; set; }

        public IPointerProvider PointerProvider { get; set; }
        public IPointerResponseSender PointerResponseSender { get; set; }
        public IDelegateHandler<bool, IPacket> PacketCallbackHandler { get; set; }


        public void Start()
        {
            WorkPool.StartPool();
        }

        public void Disconnect()
        {
            WorkPool?.StopPool();

            PacketReceiver = null;

            PacketHandler = null;

            RemoteHeap = null;

            PacketCallbackHandler = null;

            PointerResponseSender = null;

            PacketSender = null;

            WorkPool = null;

            PacketController = null;

            Stream = null;

            Connection?.Disconnect();

            Connection = null;

            BackingClient?.Dispose();

            BackingClient = null;
        }
    }
}
