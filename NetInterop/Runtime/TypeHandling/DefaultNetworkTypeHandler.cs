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

        public DefaultNetworkTypeHandler(IPointerProvider pointerProvider)
        {
            this.pointerProvider = pointerProvider ?? throw new ArgumentNullException(nameof(pointerProvider));
        }

        private readonly IDictionary<ushort, INetworkType> registeredTypes = new Dictionary<ushort, INetworkType>();
        private readonly IDictionary<Type, ushort> idRegistry = new Dictionary<Type, ushort>();

        public INetPtr<T> RegisterType<T>(ushort explicitId, IActivator<T> activator, IDeactivator<T> disposer, IPacketSerializer<T> serializer, IPacketDeserializer<T> deserializer)
        {
            Type tType = typeof(T);

            explicitId = GetId(tType, explicitId);


            INetworkType newRegistration;

            if (serializer != null && deserializer != null)
            {
                newRegistration = new DefaultSerializableNetworkType<T>(new DefaultNetworkType<T>(explicitId, pointerProvider, instantiator, disposer), deserializer, serializer);
            }
            else
            {
                newRegistration = new DefaultNetworkType<T>(explicitId, pointerProvider, instantiator, disposer);
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

        private ushort GetId(Type type, ushort explicitId)
        {
            if (explicitId == 0)
            {
                // a type must have a GUID or InteropId to be registered
                InteropIdAttribute interopId = type.GetCustomAttribute<InteropIdAttribute>();

                if (interopId is null)
                {
                    throw new InvalidOperationException($"Attempted to register {type.FullName} as a network type but the class does not implement {nameof(InteropIdAttribute)}. If this is not possible use {nameof(RegisterType)} with an {nameof(explicitId)} parameter to explicitly define it.");
                }

                explicitId = interopId.Id;
            }

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

        public bool TryGetSerializableType<T>(INetPtr<T> id, out ISerializableNetworkType<T> serializableNetworkType)
        {
            serializableNetworkType = default;
            if (TryGetType<T>(id, out var netType))
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

        public bool TryGetAmbiguousSerializableType(INetPtr id, out ISerializableNetworkType serializableNetworkType)
        {
            serializableNetworkType = default;
            if (TryGetAmbiguousType(id, out var netType))
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

        public bool TryGetAmbiguousType(INetPtr ptr, out INetworkType type)
        {
            if (registeredTypes.ContainsKey(ptr.PtrType))
            {
                type = registeredTypes[ptr.PtrType];
                return true;
            }
            type = default;
            return false;
        }

        public bool TryGetType<T>(INetPtr<T> ptr, out INetworkType<T> type)
        {
            if (registeredTypes.ContainsKey(ptr.PtrType))
            {
                INetworkType possiblyRegisteredType = registeredTypes[ptr.PtrType];

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

        public bool TryGetTypePtr<T>(out INetPtr<T> ptr)
        {
            Type t = typeof(T);
            if (idRegistry.ContainsKey(t))
            {
                ushort id = idRegistry[t];

                ptr = pointerProvider.Create<T>(id, 0);
                return true;
            }
            ptr = pointerProvider.Create<T>(0, 0);
            return false;
        }

        public bool TryGetTypePtr(Type type, out INetPtr ptr)
        {
            if (idRegistry.ContainsKey(type))
            {
                ushort id = idRegistry[type];

                ptr = pointerProvider.Create(id, 0);
                return true;
            }
            ptr = pointerProvider.Create(0, 0);
            return false;
        }


    }
}
