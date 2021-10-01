using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using NetInterop.Attributes;
using NetInterop.Transport.Core.Abstractions.Packets;
using NetInterop.Abstractions;

namespace NetInterop
{
    public class DefaultNetworkTypeHandler : INetworkTypeHandler
    {
        private readonly IPointerProvider pointerProvider;
        private readonly IDictionary<ushort, INetworkType> registeredTypes = new Dictionary<ushort, INetworkType>();
        private readonly IDictionary<Type, ushort> idRegistry = new Dictionary<Type, ushort>();

        public DefaultNetworkTypeHandler(IPointerProvider pointerProvider)
        {
            this.pointerProvider = pointerProvider ?? throw new ArgumentNullException(nameof(pointerProvider));
        }

        public INetPtr<T> RegisterType<T>(ushort explicitId, IActivator<T> activator, IDeactivator<T> disposer, IPacketSerializer<T> serializer, IPacketDeserializer<T> deserializer)
        {
            Type tType = typeof(T);

            explicitId = ValidateId(tType, explicitId);


            INetworkType newRegistration;

            if (serializer != null && deserializer != null)
            {
                newRegistration = new DefaultSerializableNetworkType<T>(new DefaultNetworkType<T>(explicitId, pointerProvider, activator, disposer), deserializer, serializer);
            }
            else
            {
                newRegistration = new DefaultNetworkType<T>(explicitId, pointerProvider, activator, disposer);
            }

            registeredTypes.Add(explicitId, newRegistration);

            idRegistry.Add(tType, explicitId);

            return pointerProvider.Create<T>(explicitId, 0);
        }

        public void Clear()
        {
            foreach (var item in registeredTypes)
            {
                item.Value.FreeAll();
            }
            registeredTypes.Clear();
        }


        public bool TryGetSerializableType<T>(out ISerializableNetworkType<T> serializableNetworkType)
        {
            serializableNetworkType = default;

            if (TryGetType<T>(out var netType))
            {
                serializableNetworkType = default;

                if (netType is ISerializableNetworkType<T> serializableType)
                {
                    serializableNetworkType = serializableType;
                    return true;
                }
            }
            return false;

        }

        public bool TryGetSerializableType<T>(INetPtr<T> typePtr, out ISerializableNetworkType<T> serializableNetworkType)
        {
            serializableNetworkType = default;
            if (TryGetType<T>(typePtr, out var netType))
            {
                serializableNetworkType = default;

                if (netType is ISerializableNetworkType<T> serializableType)
                {
                    serializableNetworkType = serializableType;
                    return true;
                }
            }
            return false;
        }

        public bool TryGetAmbiguousSerializableType(INetPtr typePtr, out ISerializableNetworkType serializableNetworkType)
        {
            serializableNetworkType = default;
            if (TryGetAmbiguousType(typePtr, out var netType))
            {
                serializableNetworkType = default;

                if (netType is ISerializableNetworkType serializableType)
                {
                    serializableNetworkType = serializableType;
                    return true;
                }
            }

            return false;
        }

        public bool TryGetAmbiguousType(INetPtr typePtr, out INetworkType type)
        {
            if (registeredTypes.ContainsKey(typePtr.PtrType))
            {
                type = registeredTypes[typePtr.PtrType];
                return true;
            }
            type = default;
            return false;
        }

        public bool TryGetType<T>(INetPtr<T> typePtr, out INetworkType<T> type)
        {
            if (registeredTypes.ContainsKey(typePtr.PtrType))
            {
                INetworkType possiblyRegisteredType = registeredTypes[typePtr.PtrType];

                if (possiblyRegisteredType is INetworkType<T> isRegisteredType)
                {
                    type = isRegisteredType;
                    return true;
                }
            }
            type = default;
            return false;
        }

        public bool TryGetType<T>(out INetworkType<T> type)
        {
            ushort id = idRegistry[typeof(T)];

            if (registeredTypes.ContainsKey(id))
            {
                INetworkType possiblyRegisteredType = registeredTypes[id];

                if (possiblyRegisteredType is INetworkType<T> isRegisteredType)
                {
                    type = isRegisteredType;
                    return true;
                }
            }

            type = default;
            return false;
        }

        public bool TryGetTypePtr<T>(out INetPtr<T> typePtr)
        {
            if (TryGetTypePtr(typeof(T), out var p))
            {
                typePtr = p.As<T>();
                return true;
            }

            typePtr = default;
            return false;
        }

        public bool TryGetTypePtr(Type type, out INetPtr typePtr)
        {
            if (idRegistry.ContainsKey(type))
            {
                ushort id = idRegistry[type];

                typePtr = pointerProvider.Create(id, 0);
                return true;
            }
            typePtr = pointerProvider.Create(0, 0);
            return false;
        }

        private ushort ValidateId(Type type, ushort explicitId)
        {
            if (explicitId == 0)
            {
                throw new InvalidOperationException($"Attempted to register {type.FullName} as a network type with either an explicit id, or {nameof(InteropIdAttribute)} with the id of 0x0000, this is not allowed. Id must be between 0x0001 and 0xFFFE.");
            }
            if (explicitId == 0xFFFF && type.IsAssignableFrom(typeof(INetPtr)) is false)
            {
                throw new InvalidOperationException($"Attempted to register {type.FullName} as a network type with either an explicit id, or {nameof(InteropIdAttribute)} with the id of 0xFFFF, this is not allowed as 0xFFFF is reserved for {nameof(NetInterop)} runtime logic. Id must be between 0x0001 and 0xFFFE.");
            }

            return explicitId;
        }
    }
}
