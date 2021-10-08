using NetInterop.Transport.Core.Abstractions.Packets;
using NetInterop.Transport.Core.Packets.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetInterop.Serialization
{
    public class ULongSerializer : BaseSerializer<ulong>
    {
        public static ULongSerializer Instance { get; private set; }

        static ULongSerializer()
        {
            if (Instance is null)
            {
                Instance = new ULongSerializer();
            }
        }
        private ULongSerializer() { }

        public override ulong CreateInstance() => default;

        public override ulong Deserialize(IPacket packet) => packet.GetULong();

        public override void DestroyInstance(ref ulong instance) { }

        public override int EstimatePacketSize(ulong value) => sizeof(ulong);

        public override void Serialize(ulong value, IPacket packetBuilder)
        {
            packetBuilder.AppendULong(value);
        }
    }
}
