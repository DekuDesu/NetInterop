using System;
using System.Collections.Generic;
using System.Text;
using NetInterop.Abstractions;
using NetInterop.Transport.Core.Abstractions.Packets;

namespace NetInterop.Runtime
{
    public class SerializableNetType<T> : ISerializableNetType<T>
    {
        private readonly INetType<T> type;
        private readonly IPacketSerializer<T> serializer;
        private readonly IPacketDeserializer<T> deserializer;

        public SerializableNetType(INetType<T> type, IPacketSerializer<T> serializer, IPacketDeserializer<T> deserializer)
        {
            this.type = type;
            this.serializer = serializer;
            this.deserializer = deserializer;
        }

        public INetPtr TypePointer => type.TypePointer;

        public T Activate() => type.Activate();

        public object AmbiguousDeserialize(IPacket packet) => deserializer.AmbiguousDeserialize(packet);

        public void AmbiguousSerialize(object value, IPacket packetBuilder) => serializer.Serialize((T)value, packetBuilder);

        public void Deactivate(ref T instance) => type.Deactivate(ref instance);

        public void Deactivate(ref object instance) => type.Deactivate(ref instance);

        public T Deserialize(IPacket packet) => deserializer.Deserialize(packet);

        public void Serialize(T value, IPacket packetBuilder) => serializer.Serialize(value, packetBuilder);

        object INetType.Activate() => type.Activate();
    }
}
