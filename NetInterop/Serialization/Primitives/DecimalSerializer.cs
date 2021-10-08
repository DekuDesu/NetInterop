using NetInterop.Transport.Core.Abstractions.Packets;
using NetInterop.Transport.Core.Packets.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetInterop.Serialization
{
    public class DecimalSerializer : BaseSerializer<decimal>
    {
        public static DecimalSerializer Instance { get; private set; }

        static DecimalSerializer()
        {
            if (Instance is null)
            {
                Instance = new DecimalSerializer();
            }
        }
        private DecimalSerializer() { }

        public override decimal CreateInstance() => default;

        public override decimal Deserialize(IPacket packet) => packet.GetDecimal();

        public override void DestroyInstance(ref decimal instance) { }

        public override int EstimatePacketSize(decimal value) => sizeof(decimal);

        public override void Serialize(decimal value, IPacket packetBuilder)
        {
            packetBuilder.AppendDecimal(value);
        }
    }
}
