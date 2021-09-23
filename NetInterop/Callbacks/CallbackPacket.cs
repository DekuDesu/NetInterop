using NetInterop.Transport.Core.Abstractions.Packets;
using NetInterop.Transport.Core.Packets.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetInterop.Callbacks
{
    public class CallbackPacket : IPacketSerializable
    {
        private readonly Action<bool, IPacket> callback;
        private readonly IDelegateHandler<bool, IPacket> packetCallbackHandler;
        private readonly IPacketSerializable packet;

        public CallbackPacket(Action<bool, IPacket> callback, IPacketSerializable packet, IDelegateHandler<bool, IPacket> packetCallbackHandler)
        {
            this.packet = packet;
            this.packetCallbackHandler = packetCallbackHandler;
            this.callback = callback;
        }

        public int EstimatePacketSize() => sizeof(ushort) + packet.EstimatePacketSize();

        public void Serialize(IPacket packetBuilder)
        {
            packetBuilder.AppendUShort(packetCallbackHandler.Register(callback));

            packet.Serialize(packetBuilder);
        }
    }
}
