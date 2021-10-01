using NetInterop.Transport.Core.Abstractions.Packets;
using NetInterop.Transport.Core.Packets.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetInterop
{
    public class CallbackResponsePacket : IPacketSerializable
    {
        private readonly ushort callbackId;
        private readonly IPacketSerializable wrappedPacket;

        public CallbackResponsePacket(ushort callbackId, IPacketSerializable wrappedPacket = null)
        {
            this.callbackId = callbackId;
            this.wrappedPacket = wrappedPacket;
        }

        public int EstimatePacketSize() => sizeof(ushort) + wrappedPacket?.EstimatePacketSize() ?? 0;

        public void Serialize(IPacket packetBuilder)
        {
            packetBuilder.AppendUShort(callbackId);
            wrappedPacket?.Serialize(packetBuilder);
        }
    }
}
