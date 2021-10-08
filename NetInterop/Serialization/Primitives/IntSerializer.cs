using NetInterop.Transport.Core.Abstractions.Packets;
using NetInterop.Transport.Core.Packets.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetInterop.Serialization
{
    public class IntSerializer : BaseSerializer<int>
    {
        public static IntSerializer Instance { get; private set; }

        static IntSerializer()
        {
            if (Instance is null)
            {
                Instance = new IntSerializer();
            }
        }
        private IntSerializer() { }

        public override int CreateInstance() => default;

        public override int Deserialize(IPacket packet) => packet.GetInt();

        public override void DestroyInstance(ref int instance) { }

        public override int EstimatePacketSize(int value) => sizeof(int);

        public override void Serialize(int value, IPacket packetBuilder)
        {
            packetBuilder.AppendInt(value);
        }
    }
}
