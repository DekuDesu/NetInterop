using NetInterop.Transport.Core.Abstractions.Packets;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetInterop
{
    public class DefaultDeserializer<TResult> : IPacketDeserializer<TResult>
    {
        private readonly Func<IPacket, TResult> deserializerFunc;

        public DefaultDeserializer(Func<IPacket, TResult> deserializerFunc)
        {
            this.deserializerFunc = deserializerFunc ?? throw new ArgumentNullException(nameof(deserializerFunc));
        }

        public object AmbiguousDeserialize(IPacket packet) => this.Deserialize(packet);

        public TResult Deserialize(IPacket packet)
        {
            return deserializerFunc(packet);
        }
    }
}
