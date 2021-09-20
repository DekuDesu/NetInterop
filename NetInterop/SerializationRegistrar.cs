using NetInterop.Transport.Core.Abstractions.Packets;
using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.Concurrent;
namespace NetInterop
{
    public class SerializationRegistrar<TPacket> : ISerializationRegistrar<TPacket> where TPacket : Enum, IConvertible
    {
        private readonly IDictionary<Type, object> deserializers = new ConcurrentDictionary<Type, object>();
        private readonly IDictionary<Type, object> serializers = new ConcurrentDictionary<Type, object>();

        public void Register<TResult>(IPacketSerializer<TPacket, TResult> serializer)
        {
            Type resultType = typeof(TResult);

            // intentional lack of error checking here, we should throw an error if more than one serializer gets added for the same result type
            serializers.Add(resultType, serializer);
        }

        public void Register<TResult>(IPacketDeserializer<TPacket, TResult> deserializer)
        {
            Type resultType = typeof(TResult);

            // intentional lack of error checking here, we should throw an error if more than one serializer gets added for the same result type
            serializers.Add(resultType, deserializer);
        }

        public void Serialize<T>(T value, IPacket<TPacket> packet)
        {
            Type tType = typeof(T);
            if (serializers.ContainsKey(tType))
            {
                object potentialSerializer = serializers[tType];

                if (potentialSerializer is IPacketSerializer<TPacket, T> serializer)
                {
                    serializer.Serialize(value, packet);
                    return;
                }
            }

            throw new InvalidOperationException($"Attempted to serialize a {tType.FullName}, but no serializer has been registered for {tType.FullName}");
        }

        public T Deserialize<T>(IPacket<TPacket> packet)
        {
            Type tType = typeof(T);

            if (deserializers.ContainsKey(tType))
            {
                object potentialSerializer = serializers[tType];

                if (potentialSerializer is IPacketDeserializer<TPacket, T> deserializer)
                {
                    return deserializer.Deserialize(packet);
                }
            }

            throw new InvalidOperationException($"Attempted to deserialize a packet into a {tType.FullName}, but no deserializer has been registered for {tType.FullName}");
        }
    }
}
