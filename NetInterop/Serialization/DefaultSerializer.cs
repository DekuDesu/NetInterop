using NetInterop.Transport.Core.Abstractions.Packets;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetInterop
{
    public class DefaultSerializer<T> : IPacketSerializer<T>
    {
        private readonly Action<T, IPacket> serializeAction;

        public DefaultSerializer(Action<T, IPacket> serializeAction)
        {
            this.serializeAction = serializeAction ?? throw new ArgumentNullException(nameof(serializeAction));
        }

        public void Serialize(T value, IPacket packetBuilder)
        {
            serializeAction(value, packetBuilder);
        }
    }
}
