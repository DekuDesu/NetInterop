using NetInterop.Transport.Core.Abstractions.Packets;
using NetInterop.Transport.Core.Packets.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetInterop.Serialization
{
    public class FloatSerializer : BaseSerializer<float>
    {
        public static FloatSerializer Instance { get; private set; }

        static FloatSerializer()
        {
            if (Instance is null)
            {
                Instance = new FloatSerializer();
            }
        }
        private FloatSerializer() { }

        public override float CreateInstance() => default;

        public override float Deserialize(IPacket packet) => packet.GetFloat();

        public override void DestroyInstance(ref float instance) { }

        public override int EstimatePacketSize(float value) => sizeof(float);

        public override void Serialize(float value, IPacket packetBuilder)
        {
            packetBuilder.AppendFloat(value);
        }
    }
}
