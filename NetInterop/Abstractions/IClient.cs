using NetInterop.Transport.Core.Abstractions;
using NetInterop.Transport.Core.Abstractions.Connections;
using NetInterop.Transport.Core.Abstractions.Packets;
using NetInterop.Transport.Core.Abstractions.Runtime;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetInterop.Abstractions
{
    /// <summary>
    /// A remote connection to a <see cref="NetInterop"/> client/server
    /// </summary>
    public interface IClient
    {
        /// <summary>
        /// Controls the connection to the remote client/server
        /// </summary>
        IConnection Connection { get; set; }

        /// <summary>
        /// The raw backing stream for communication between this client and the remote client/server
        /// </summary>
        IStream<byte> Stream { get; set; }

        /// <summary>
        /// The registered network types that can be sent between this client and the remote client
        /// </summary>
        ITypeHandler Types { get; set; }

        /// <summary>
        /// The registred methods that can be invoked on this client of the remote client
        /// </summary>
        IMethodHandler Methods { get; set; }

        /// <summary>
        /// The object heap that controls creating, destroying, and managing network types on the remote client
        /// </summary>
        INetworkHeap RemoteHeap { get; set; }

        /// <summary>
        /// Controls recieving and sending raw <see cref="IPacket"/>s
        /// </summary>
        IPacketController PacketController { get; set; }

        /// <summary>
        /// Controls receiving packet data and dispatching it to the <see cref="IPacketHandler"/>
        /// </summary>
        IPacketReceiver PacketReceiver { get; set; }

        /// <summary>
        /// Controls sending serializable types and messages to the remote client
        /// </summary>
        IPacketSender PacketSender { get; set; }

        /// <summary>
        /// Controls the threads assigned to this client for serialization, deserialization, and runtime tasks
        /// </summary>
        IWorkPool WorkPool { get; set; }
        IPointerProvider PointerProvider { get; set; }
        IPointerResponseSender PointerResponseSender { get; set; }
        IDelegateHandler<bool, IPacket> PacketCallbackHandler { get; set; }

        void Start();
        void Disconnect();
    }
}
