using NetInterop.Transport.Core.Abstractions.Packets;
using NetInterop.Transport.Core.Packets.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetInterop.Serialization
{
    public class LongSerializer : BaseSerializer<long>
    {
        public static LongSerializer Instance { get; private set; }

        static LongSerializer()
        {
            if (Instance is null)
            {
                Instance = new LongSerializer();
            }
        }
        private LongSerializer() { }

        public override long CreateInstance() => default;

        public override long Deserialize(IPacket packet) => packet.GetLong();

        public override void DestroyInstance(ref long instance) { }

        public override int EstimatePacketSize(long value) => sizeof(long);

        public override void Serialize(long value, IPacket packetBuilder)
        {
            packetBuilder.AppendLong(value);
        }
    }
}
