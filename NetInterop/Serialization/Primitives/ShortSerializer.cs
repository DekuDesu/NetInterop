using NetInterop.Transport.Core.Abstractions.Packets;
using NetInterop.Transport.Core.Packets.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetInterop.Serialization
{
    public class ShortSerializer : BaseSerializer<short>
    {
        public static ShortSerializer Instance { get; private set; }

        static ShortSerializer()
        {
            if (Instance is null)
            {
                Instance = new ShortSerializer();
            }
        }
        private ShortSerializer() { }

        public override short CreateInstance() => default;

        public override short Deserialize(IPacket packet) => packet.GetShort();

        public override void DestroyInstance(ref short instance) { }

        public override int EstimatePacketSize(short value) => sizeof(short);

        public override void Serialize(short value, IPacket packetBuilder)
        {
            packetBuilder.AppendShort(value);
        }
    }
}
