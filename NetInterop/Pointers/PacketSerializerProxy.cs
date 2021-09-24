using NetInterop.Transport.Core.Abstractions.Packets;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetInterop
{
    public class PacketSerializerProxy : IPacketSerializable
    {
        private readonly object value;
        private readonly IPacketSerializer serializer;

        public PacketSerializerProxy(object value, IPacketSerializer serializer)
        {
            this.value = value;
            this.serializer = serializer;
        }

        public int EstimatePacketSize() => sizeof(long); // this completely arbitrary

        public void Serialize(IPacket packetBuilder)
        {
            serializer.AmbiguousSerialize(value, packetBuilder);
        }
    }
}
