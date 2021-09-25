using NetInterop.Transport.Core.Abstractions.Packets;
using NetInterop.Transport.Core.Packets.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetInterop
{
    public class PointerResponsePacket<TPacket> : IPacketSerializable<TPacket> where TPacket : Enum, IConvertible
    {
        private readonly bool goodResponse;
        private readonly IPacketSerializable data;

        public PointerResponsePacket(bool goodResponse, IPacketSerializable data = null)
        {
            this.goodResponse = goodResponse;
            this.data = data;
        }

        public TPacket PacketType { get; }

        public int EstimatePacketSize() => (sizeof(byte) * 2) + (data?.EstimatePacketSize() ?? 0);

        public void Serialize(IPacket packetBuilder)
        {
            // this packet should be wrapped with a PointerOperationPacket so it is dispatched from the P
            PointerResponses response = goodResponse ? PointerResponses.GoodResponse : PointerResponses.BadResponse;

            packetBuilder.AppendByte((byte)response);

            data?.Serialize(packetBuilder);
        }
    }
}
