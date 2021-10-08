using NetInterop.Transport.Core.Abstractions.Packets;
using NetInterop.Transport.Core.Packets.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetInterop.Serialization
{
    public class SByteSerializer : BaseSerializer<sbyte>
    {
        public static SByteSerializer Instance { get; private set; }

        static SByteSerializer()
        {
            if (Instance is null)
            {
                Instance = new SByteSerializer();
            }
        }
        private SByteSerializer() { }

        public override sbyte CreateInstance() => default;

        public override sbyte Deserialize(IPacket packet) => packet.GetSByte();

        public override void DestroyInstance(ref sbyte instance) { }

        public override int EstimatePacketSize(sbyte value) => sizeof(sbyte);

        public override void Serialize(sbyte value, IPacket packetBuilder)
        {
            packetBuilder.AppendSByte(value);
        }
    }
}
