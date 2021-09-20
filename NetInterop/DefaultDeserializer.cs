using NetInterop.Transport.Core.Abstractions.Packets;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetInterop
{
    public class DefaultDeserializer<TPacket, TResult> : IPacketDeserializer<TPacket, TResult> where TPacket : Enum, IConvertible
    {
        private readonly Func<IPacket<TPacket>, TResult> deserializerFunc;

        public DefaultDeserializer(Func<IPacket<TPacket>, TResult> deserializerFunc)
        {
            this.deserializerFunc = deserializerFunc ?? throw new ArgumentNullException(nameof(deserializerFunc));
        }

        public TResult Deserialize(IPacket<TPacket> packet)
        {
            return deserializerFunc(packet);
        }
    }
}
