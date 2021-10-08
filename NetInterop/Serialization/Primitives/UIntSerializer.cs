using NetInterop.Transport.Core.Abstractions.Packets;
using NetInterop.Transport.Core.Packets.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetInterop.Serialization
{
    public class UIntSerializer : BaseSerializer<uint>
    {
        public static UIntSerializer Instance { get; private set; }

        static UIntSerializer()
        {
            if (Instance is null)
            {
                Instance = new UIntSerializer();
            }
        }
        private UIntSerializer() { }

        public override uint CreateInstance() => default;

        public override uint Deserialize(IPacket packet) => packet.GetUInt();

        public override void DestroyInstance(ref uint instance) { }

        public override int EstimatePacketSize(uint value) => sizeof(uint);

        public override void Serialize(uint value, IPacket packetBuilder)
        {
            packetBuilder.AppendUInt(value);
        }
    }
}
