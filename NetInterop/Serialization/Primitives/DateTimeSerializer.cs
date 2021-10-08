using NetInterop.Transport.Core.Abstractions.Packets;
using NetInterop.Transport.Core.Packets.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetInterop.Serialization
{
    public class DateTimeSerializer : BaseSerializer<DateTime>
    {
        public static DateTimeSerializer Instance { get; private set; }

        static DateTimeSerializer()
        {
            if (Instance is null)
            {
                Instance = new DateTimeSerializer();
            }
        }
        private DateTimeSerializer() { }

        public override DateTime CreateInstance() => default;

        public override DateTime Deserialize(IPacket packet) => packet.GetDateTime();

        public override void DestroyInstance(ref DateTime instance) { }

        public override int EstimatePacketSize(DateTime value) => sizeof(long);

        public override void Serialize(DateTime value, IPacket packetBuilder)
        {
            packetBuilder.AppendDateTime(value);
        }
    }
}
