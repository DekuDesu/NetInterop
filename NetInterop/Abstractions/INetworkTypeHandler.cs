using NetInterop.Abstractions;
using NetInterop.Transport.Core.Abstractions.Packets;
using System;

namespace NetInterop
{
    public interface INetworkTypeHandler
    {
        bool TryGetAmbiguousType(INetPtr ptr, out INetworkType type);
        bool TryGetType<T>(INetPtr<T> ptr, out INetworkType<T> type);
        bool TryGetType<T>(out INetworkType<T> type);

        bool TryGetSerializableType<T>(out ISerializableNetworkType<T> serializableNetworkType);
        bool TryGetSerializableType<T>(INetPtr<T> id, out ISerializableNetworkType<T> serializableNetworkType);
        bool TryGetAmbiguousSerializableType(INetPtr id, out ISerializableNetworkType serializableNetworkType);

        bool TryGetTypePtr<T>(out INetPtr<T> ptr);
        bool TryGetTypePtr(Type type, out INetPtr ptr);

        INetPtr<T> RegisterType<T>(ushort interopId, IActivator<T> activator, IDeactivator<T> disposer, IPacketSerializer<T> serializer, IPacketDeserializer<T> deserializer);

        void Clear();
    }
}