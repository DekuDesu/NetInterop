using NetInterop.Transport.Core.Abstractions.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetInterop.Transport.Core.Packets.Extensions;

namespace NetInterop.Example.Serializers
{
    public class UTF8Serializer : IPacketSerializer<string>, IPacketDeserializer<string>
    {
        public static UTF8Serializer Instance { get; private set; }
        static UTF8Serializer()
        {
            Instance ??= new UTF8Serializer();
        }
        private UTF8Serializer()
        {

        }

        public object AmbiguousDeserialize(IPacket packet) => packet.GetString(System.Text.Encoding.UTF8);

        public void AmbiguousSerialize(object value, IPacket packetBuilder) => Serialize((string)value, packetBuilder);

        public string Deserialize(IPacket packet) => packet.GetString(System.Text.Encoding.UTF8);

        public int EstimatePacketSize(string value) => Encoding.UTF8.GetByteCount(value) + sizeof(int);

        public void Serialize(string value, IPacket packetBuilder) => packetBuilder.AppendString(value, System.Text.Encoding.UTF8);
    }
}
