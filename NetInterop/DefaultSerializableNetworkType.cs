using NetInterop.Transport.Core.Abstractions.Packets;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetInterop
{
    public class DefaultSerializableNetworkType<T> : ISerializableNetworkType<T>
    {
        private readonly INetworkType<T> networkType;
        private readonly IPacketDeserializer<T> deserializer;
        private readonly IPacketSerializer<T> serializer;

        public DefaultSerializableNetworkType(INetworkType<T> networkType, IPacketDeserializer<T> deserializer, IPacketSerializer<T> serializer)
        {
            this.networkType = networkType;
            this.deserializer = deserializer;
            this.serializer = serializer;
        }

        public ushort Id => networkType.Id;

        public INetPtr AllocPtr() => networkType.AllocPtr();

        public object AmbiguousDeserialize(IPacket packet) => deserializer.AmbiguousDeserialize(packet);

        public T Deserialize(IPacket packet) => deserializer.Deserialize(packet);

        public void Free(INetPtr ptr) => networkType.Free(ptr);

        public void FreeAll() => networkType.FreeAll();

        public T GetPtr(INetPtr<T> ptr) => networkType.GetPtr(ptr);

        public object GetPtr(INetPtr ptr) => networkType.GetPtr(ptr);

        public void Serialize(T value, IPacket packetBuilder) => serializer.Serialize(value, packetBuilder);

        public void AmbiguousSerialize(object value, IPacket packetBuilder)
        {
            if (value is T isT)
            {
                serializer.Serialize(isT, packetBuilder);
                return;
            }
            throw new InvalidOperationException($"Failed to serialize ambiguous object {value.GetType().FullName} to {typeof(T).FullName}");
        }

        public void SetPtr(INetPtr<T> ptr, T value) => networkType.SetPtr(ptr, value);

        public void SetPtr(INetPtr ptr, object value) => networkType.SetPtr(ptr, value);
    }
}
