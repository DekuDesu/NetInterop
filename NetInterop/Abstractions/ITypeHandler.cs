using NetInterop.Transport.Core.Abstractions.Packets;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetInterop.Abstractions
{
    public interface ITypeHandler
    {
        int Count { get; }
        bool TryGetType<T>(out IType<T> netType);
        bool TryGetType(Type type, out IType netType);
        bool TryGetType(INetPtr typePtr, out IType netType);
        bool TryGetType<T>(INetPtr typePtr, out IType<T> netType);

        bool TryGetSerializableType<T>(out ISerializableType<T> netType);
        bool TryGetSerializableType(Type type, out ISerializableType netType);
        bool TryGetSerializableType(INetPtr typePtr, out ISerializableType netType);
        bool TryGetSerializableType<T>(INetPtr typePtr, out ISerializableType<T> netType);

        INetPtr<T> RegisterType<T>(ushort interopId, IActivator<T> activator, IDeactivator<T> deactivator, IPacketSerializer<T> serializer, IPacketDeserializer<T> deserializer);
    }
}
