using NetInterop.Transport.Core.Abstractions.Packets;
using NetInterop.Transport.Core.Packets.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetInterop.Serialization
{
    public class ByteSerializer : BaseSerializer<byte>
    {
        public static ByteSerializer Instance { get; private set; }

        static ByteSerializer()
        {
            if (Instance is null)
            {
                Instance = new ByteSerializer();
            }
        }
        private ByteSerializer() { }

        public override byte CreateInstance() => default;

        public override byte Deserialize(IPacket packet) => packet.GetByte();

        public override void DestroyInstance(ref byte instance) { }

        public override int EstimatePacketSize(byte value) => 1;

        public override void Serialize(byte value, IPacket packetBuilder)
        {
            packetBuilder.AppendByte(value);
        }
    }
}
