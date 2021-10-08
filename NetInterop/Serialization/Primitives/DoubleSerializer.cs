using NetInterop.Transport.Core.Abstractions.Packets;
using NetInterop.Transport.Core.Packets.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetInterop.Serialization
{
    public class DoubleSerializer : BaseSerializer<double>
    {
        public static DoubleSerializer Instance { get; private set; }

        static DoubleSerializer()
        {
            if (Instance is null)
            {
                Instance = new DoubleSerializer();
            }
        }
        private DoubleSerializer() { }

        public override double CreateInstance() => default;

        public override double Deserialize(IPacket packet) => packet.GetDouble();

        public override void DestroyInstance(ref double instance) { }

        public override int EstimatePacketSize(double value) => sizeof(double);

        public override void Serialize(double value, IPacket packetBuilder)
        {
            packetBuilder.AppendDouble(value);
        }
    }
}
