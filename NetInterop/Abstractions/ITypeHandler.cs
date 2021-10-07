using NetInterop.Transport.Core.Abstractions.Packets;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetInterop.Abstractions
{
    public interface ITypeHandler
    {
        bool TryGetType<T>(out INetType<T> netType);
        bool TryGetType(Type type, out INetType netType);
        bool TryGetType(INetPtr typePtr, out INetType netType);
        bool TryGetType<T>(INetPtr typePtr, out INetType<T> netType);

        bool TryGetSerializableType<T>(out ISerializableNetType<T> netType);
        bool TryGetSerializableType(Type type, out ISerializableNetType netType);
        bool TryGetSerializableType(INetPtr typePtr, out ISerializableNetType netType);
        bool TryGetSerializableType<T>(INetPtr typePtr, out ISerializableNetType<T> netType);

        INetPtr<T> RegisterType<T>(ushort interopId, IActivator<T> activator, IDeactivator<T> deactivator, IPacketSerializer<T> serializer, IPacketDeserializer<T> deserializer);
    }
}
