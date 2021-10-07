using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using NetInterop.Abstractions;
using NetInterop.Transport.Core.Abstractions.Packets;

namespace NetInterop.Runtime
{
    public class NetTypeHandler : ITypeHander
    {
        private readonly IPointerProvider pointerProvider;
        private readonly IDictionary<ushort, INetType> types = new ConcurrentDictionary<ushort, INetType>();
        private readonly IDictionary<Type, ushort> typeMap = new ConcurrentDictionary<Type, ushort>();

        public NetTypeHandler(IPointerProvider pointerProvider)
        {
            this.pointerProvider = pointerProvider;
        }

        public INetPtr<T> RegisterType<T>(ushort interopId, IActivator<T> activator, IDeactivator<T> deactivator, IPacketSerializer<T> serializer, IPacketDeserializer<T> deserializer)
        {
            INetPtr<T> ptr = pointerProvider.Create<T>(interopId, 0);

            if (serializer != null && deserializer != null)
            {
                types.Add(interopId, new SerializableNetType<T>(new NetType<T>(ptr, activator, deactivator), serializer, deserializer));
            }
            else
            {
                types.Add(interopId, new NetType<T>(ptr, activator, deactivator));
            }

            typeMap.Add(typeof(T),interopId);

            return ptr;
        }

        public bool TryGetSerializableType(INetPtr typePtr, out ISerializableNetType netType)
        {
            if (types.TryGetValue(typePtr.PtrType, out INetType possiblySerializable))
            {
                if (possiblySerializable is ISerializableNetType isSerializable)
                {
                    netType = isSerializable;
                    return true;
                }
            }
            netType = default;
            return false;
        }

        public bool TryGetSerializableType<T>(INetPtr typePtr, out ISerializableNetType<T> netType)
        {
            if (types.TryGetValue(typePtr.PtrType, out INetType possiblySerializable))
            {
                if (possiblySerializable is ISerializableNetType<T> isSerializable)
                {
                    netType = isSerializable;
                    return true;
                }
            }
            netType = default;
            return false;
        }

        public bool TryGetSerializableType(Type type, out ISerializableNetType netType)
        {
            if (typeMap.TryGetValue(type, out ushort id))
            {
                if (types.TryGetValue(id, out INetType possiblySerializable))
                {
                    if (possiblySerializable is ISerializableNetType isSerializable)
                    {
                        netType = isSerializable;
                        return true;
                    }
                }
            }
            netType = default;
            return false;
        }

        public bool TryGetSerializableType<T>(out ISerializableNetType<T> netType)
        {
            if (typeMap.TryGetValue(typeof(T), out ushort id))
            {
                if (types.TryGetValue(id, out INetType possiblyTyped))
                {
                    if (possiblyTyped is ISerializableNetType<T> isProperlyTyped)
                    {
                        netType = isProperlyTyped;
                        return true;
                    }
                }
            }
            netType = default;
            return false;
        }

        public bool TryGetType(INetPtr typePtr, out INetType netType) => types.TryGetValue(typePtr.PtrType, out netType);

        public bool TryGetType<T>(INetPtr typePtr, out INetType<T> netType)
        {
            if (types.TryGetValue(typePtr.PtrType, out INetType possiblyTyped))
            {
                if (possiblyTyped is INetType<T> isProperlyTyped)
                {
                    netType = isProperlyTyped;
                    return true;
                }
            }
            netType = default;
            return false;
        }

        public bool TryGetType(Type type, out INetType netType)
        {
            if (typeMap.TryGetValue(type, out ushort id))
            {
                if (types.TryGetValue(id, out netType))
                {
                    return true;
                }
            }
            netType = default;
            return false;
        }

        public bool TryGetType<T>(out INetType<T> netType)
        {
            if (typeMap.TryGetValue(typeof(T), out ushort id))
            {
                if (types.TryGetValue(id, out INetType possiblyTyped))
                {
                    if (possiblyTyped is INetType<T> isProperlyTyped)
                    {
                        netType = isProperlyTyped;
                        return true;
                    }
                }
            }
            netType = default;
            return false;
        }
    }
}
