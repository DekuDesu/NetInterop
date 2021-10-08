using NetInterop.Transport.Core.Abstractions.Packets;
using NetInterop.Transport.Core.Packets.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetInterop.Serialization
{
    public class UShortSerializer : BaseSerializer<ushort>
    {
        public static UShortSerializer Instance { get; private set; }

        static UShortSerializer()
        {
            if (Instance is null)
            {
                Instance = new UShortSerializer();
            }
        }
        private UShortSerializer() { }

        public override ushort CreateInstance() => default;

        public override ushort Deserialize(IPacket packet) => packet.GetUShort();

        public override void DestroyInstance(ref ushort instance) { }

        public override int EstimatePacketSize(ushort value) => sizeof(ushort);

        public override void Serialize(ushort value, IPacket packetBuilder)
        {
            packetBuilder.AppendUShort(value);
        }
    }
}
