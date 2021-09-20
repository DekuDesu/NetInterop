using NetInterop.Transport.Core.Abstractions.Packets;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetInterop
{
    public class DefaultSerializer<TPacket, T> : IPacketSerializer<TPacket, T> where TPacket : Enum, IConvertible
    {
        private readonly Action<T, IPacket<TPacket>> serializeAction;

        public DefaultSerializer(Action<T, IPacket<TPacket>> serializeAction)
        {
            this.serializeAction = serializeAction ?? throw new ArgumentNullException(nameof(serializeAction));
        }

        public void Serialize(T value, IPacket<TPacket> packetBuilder)
        {
            serializeAction(value, packetBuilder);
        }
    }
}
