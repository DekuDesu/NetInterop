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
    public class ModifierGroup
    {
        public IDependencyModifier<IObjectHeap> ObjectHeap { get; set; } = new DependencyModifierStub<IObjectHeap>();
        public IDependencyModifier<ITypeHandler> TypeHandler { get; set; } = new DependencyModifierStub<ITypeHandler>();
        public IDependencyModifier<IMethodHandler> MethodHandler { get; set; } = new DependencyModifierStub<IMethodHandler>();
        public IDependencyModifier<IPointerProvider> PointerProvider { get; set; } = new DependencyModifierStub<IPointerProvider>();
        public IDependencyModifier<IConnection> Connection { get; set; } = new DependencyModifierStub<IConnection>();
        public IDependencyModifier<IConnectionManager> ConnectionManager { get; set; } = new DependencyModifierStub<IConnectionManager>();
        public IDependencyModifier<IClient> Client { get; set; } = new DependencyModifierStub<IClient>();
        public IDependencyModifier<IWorkPool> WorkPool { get; set; } = new DependencyModifierStub<IWorkPool>();
        public IDependencyModifier<IStream<byte>> Stream { get; set; } = new DependencyModifierStub<IStream<byte>>();
        public IDependencyModifier<IPacketController> PacketController { get; set; } = new DependencyModifierStub<IPacketController>();
        public IDependencyModifier<IPacketReceiver> PacketReceiver { get; set; } = new DependencyModifierStub<IPacketReceiver>();
        public IDependencyModifier<IPacketSender> PacketSender { get; set; } = new DependencyModifierStub<IPacketSender>();
        public IDependencyModifier<IPointerResponseSender> PointerRepsonseSender { get; set; } = new DependencyModifierStub<IPointerResponseSender>();
        public IDependencyModifier<IDelegateHandler<bool, IPacket>> CallbackHandler { get; set; } = new DependencyModifierStub<IDelegateHandler<bool, IPacket>>();
        public IDependencyModifier<IRemoteHeap> RemoteHeap { get; set; } = new DependencyModifierStub<IRemoteHeap>();
        public IDependencyModifier<IPacketHandler> PacketHandler { get; set; } = new DependencyModifierStub<IPacketHandler>();
        public IDependencyModifier<IClientHandler<TcpClient>> ClientHandler { get; set; } = new DependencyModifierStub<IClientHandler<TcpClient>>();
        public IDependencyModifier<IServer> Server { get; set; } = new DependencyModifierStub<IServer>();
    }
}
