using NetInterop.Transport.Core.Abstractions.Packets;
using System;

namespace NetInterop
{
    public interface ISerializationRegistrar<TPacket> where TPacket : Enum, IConvertible
    {
        T Deserialize<T>(IPacket<TPacket> packet);
        void Register<TResult>(IPacketDeserializer<TPacket, TResult> deserializer);
        void Register<TResult>(IPacketSerializer<TPacket, TResult> serializer);
        void Serialize<T>(T value, IPacket<TPacket> packet);
    }
}