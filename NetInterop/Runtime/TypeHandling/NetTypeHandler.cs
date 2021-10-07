using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using NetInterop.Abstractions;
using NetInterop.Transport.Core.Abstractions.Packets;

namespace NetInterop.Runtime
{
    public class NetTypeHandler : ITypeHandler
    {
        private readonly IPointerProvider pointerProvider;
        private readonly IDictionary<ushort, IType> types = new ConcurrentDictionary<ushort, IType>();
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

        public bool TryGetSerializableType(INetPtr typePtr, out ISerializableType netType)
        {
            if (types.TryGetValue(typePtr.PtrType, out IType possiblySerializable))
            {
                if (possiblySerializable is ISerializableType isSerializable)
                {
                    netType = isSerializable;
                    return true;
                }
            }
            netType = default;
            return false;
        }

        public bool TryGetSerializableType<T>(INetPtr typePtr, out ISerializableType<T> netType)
        {
            if (types.TryGetValue(typePtr.PtrType, out IType possiblySerializable))
            {
                if (possiblySerializable is ISerializableType<T> isSerializable)
                {
                    netType = isSerializable;
                    return true;
                }
            }
            netType = default;
            return false;
        }

        public bool TryGetSerializableType(Type type, out ISerializableType netType)
        {
            if (typeMap.TryGetValue(type, out ushort id))
            {
                if (types.TryGetValue(id, out IType possiblySerializable))
                {
                    if (possiblySerializable is ISerializableType isSerializable)
                    {
                        netType = isSerializable;
                        return true;
                    }
                }
            }
            netType = default;
            return false;
        }

        public bool TryGetSerializableType<T>(out ISerializableType<T> netType)
        {
            if (typeMap.TryGetValue(typeof(T), out ushort id))
            {
                if (types.TryGetValue(id, out IType possiblyTyped))
                {
                    if (possiblyTyped is ISerializableType<T> isProperlyTyped)
                    {
                        netType = isProperlyTyped;
                        return true;
                    }
                }
            }
            netType = default;
            return false;
        }

        public bool TryGetType(INetPtr typePtr, out IType netType) => types.TryGetValue(typePtr.PtrType, out netType);

        public bool TryGetType<T>(INetPtr typePtr, out IType<T> netType)
        {
            if (types.TryGetValue(typePtr.PtrType, out IType possiblyTyped))
            {
                if (possiblyTyped is IType<T> isProperlyTyped)
                {
                    netType = isProperlyTyped;
                    return true;
                }
            }
            netType = default;
            return false;
        }

        public bool TryGetType(Type type, out IType netType)
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

        public bool TryGetType<T>(out IType<T> netType)
        {
            if (typeMap.TryGetValue(typeof(T), out ushort id))
            {
                if (types.TryGetValue(id, out IType possiblyTyped))
                {
                    if (possiblyTyped is IType<T> isProperlyTyped)
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
