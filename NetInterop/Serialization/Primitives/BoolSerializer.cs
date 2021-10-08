using NetInterop.Transport.Core.Abstractions.Packets;
using NetInterop.Transport.Core.Packets.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetInterop.Serialization
{
    public class BoolSerializer : BaseSerializer<bool>
    {
        public static BoolSerializer Instance { get; private set; }

        static BoolSerializer()
        {
            if (Instance is null)
            {
                Instance = new BoolSerializer();
            }
        }
        private BoolSerializer() { }

        public override bool CreateInstance() => default;

        public override bool Deserialize(IPacket packet) => packet.GetBool();

        public override void DestroyInstance(ref bool instance) { }

        public override int EstimatePacketSize(bool value) => sizeof(bool);

        public override void Serialize(bool value, IPacket packetBuilder)
        {
            packetBuilder.AppendBool(value);
        }
    }
}
