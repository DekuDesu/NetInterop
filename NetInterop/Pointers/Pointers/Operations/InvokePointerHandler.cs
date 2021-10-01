using NetInterop.Transport.Core.Abstractions.Packets;
using NetInterop.Transport.Core.Packets.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetInterop
{
    public class InvokePointerHandler : IPacketHandler<PointerOperations>
    {
        private readonly INetworkMethodHandler methodHandler;
        private readonly IPointerProvider pointerProvider;
        private readonly IPointerResponseSender sender;

        public InvokePointerHandler(IPointerProvider pointerProvider, IPointerResponseSender sender, INetworkMethodHandler methodHandler)
        {
            this.methodHandler = methodHandler ?? throw new ArgumentNullException(nameof(methodHandler));
            this.pointerProvider = pointerProvider ?? throw new ArgumentNullException(nameof(pointerProvider));
            this.sender = sender ?? throw new ArgumentNullException(nameof(sender));
        }

        public PointerOperations PacketType { get; } = PointerOperations.Invoke;

        public void Handle(IPacket packet)
        {
            // get the callback id for responses
            ushort callbackId = packet.GetUShort();

            // get the method ptr from the packet
            INetPtr methodPtr = pointerProvider.Deserialize(packet);

            if (methodPtr is null)
            {
                sender.SendBadResponse(callbackId);
            }

            sender.SendResponse(callbackId, true, new InvokeMethodPacketProxy(methodPtr, methodHandler, packet));
        }
    }

    public class InvokeMethodPacketProxy : IPacketSerializable
    {
        private readonly INetPtr methodPtr;
        private readonly INetworkMethodHandler methodHandler;
        private readonly IPacket packet;

        public InvokeMethodPacketProxy(INetPtr methodPtr, INetworkMethodHandler methodHandler, IPacket packet)
        {
            this.methodPtr = methodPtr;
            this.methodHandler = methodHandler;
            this.packet = packet;
        }

        public int EstimatePacketSize() => sizeof(long);

        public void Serialize(IPacket packetBuilder)
        {
            methodHandler.Invoke(methodPtr, packet, packetBuilder);
        }
    }
}
