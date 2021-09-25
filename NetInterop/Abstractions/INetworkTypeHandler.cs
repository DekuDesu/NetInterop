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

        [System.Obsolete("Use RegisterType<T> instead for type safety, if available. This is remains for niche compatibility.")]
        ushort RegisterType(Type type);
        [System.Obsolete("Use RegisterType<T> instead for type safety, if available. This is remains for niche compatibility.")]
        ushort RegisterType(Type type, ushort explicitId);
        [System.Obsolete("Use RegisterType<T> instead for type safety, if available. This is remains for niche compatibility.")]
        ushort RegisterType(Type type, ushort explicitId, object instantiator);


        INetPtr<T> RegisterType<T>(ushort explicitId);
        INetPtr<T> RegisterType<T>(Func<T> instantiator);
        INetPtr<T> RegisterType<T>(Action<T> disposer);
        INetPtr<T> RegisterType<T>(Func<T> instantiator, Action<T> disposer);
        INetPtr<T> RegisterType<T>(ushort explicitId, IPacketSerializer<T> serializer, IPacketDeserializer<T> deserializer);
        INetPtr<T> RegisterType<T>(ushort explicitId, Func<T> instantiator, Action<T> disposer);
        INetPtr<T> RegisterType<T>(ushort explicitId, Func<T> instantiator);
        INetPtr<T> RegisterType<T>(ushort explicitId, Func<T> instantiator, Action<T> disposer, IPacketSerializer<T> serializer, IPacketDeserializer<T> deserializer);

        void Clear();
    }
}