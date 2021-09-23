using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using NetInterop.Attributes;
using NetInterop.Transport.Core.Abstractions.Packets;

namespace NetInterop
{
    public class DefaultNetworkTypeHandler : INetworkTypeHandler
    {
        private readonly IDictionary<ushort, INetworkType> registeredTypes = new Dictionary<ushort, INetworkType>();
        private readonly IDictionary<Type, ushort> idRegistry = new Dictionary<Type, ushort>();

        public ushort RegisterType(Type type, ushort explicitId, object instantiator)
        {
            explicitId = GetId(type, explicitId);

            // create the network register type
            var constructedGeneric = typeof(DefaultNetworkType<>).MakeGenericType(new Type[] { type });

            Type instantiatorType = instantiator.GetType().GetGenericArguments().FirstOrDefault() ?? type;

            if (instantiatorType != type)
            {
                throw new InvalidCastException($"Attempted to assign an instantiator for the type {type.FullName} that does not have the same type arguments as expected. Expected: {type.FullName} Actual: {instantiatorType.FullName}");
            }

            var newRegistration = (INetworkType)Activator.CreateInstance(constructedGeneric, new object[] { explicitId, instantiator });

            registeredTypes.Add(explicitId, newRegistration);

            return explicitId;
        }
        public ushort RegisterType(Type type) => RegisterType(type, 0, null);
        public ushort RegisterType(Type type, ushort explicitId) => RegisterType(type, explicitId, null);

        public ushort RegisterType<T>(ushort explicitId) => RegisterType<T>(explicitId, null, null, null, null);
        public ushort RegisterType<T>(Func<T> instantiator) => RegisterType<T>(0, instantiator, null, null, null);
        public ushort RegisterType<T>(Action<T> disposer) => RegisterType<T>(0, null, disposer, null, null);
        public ushort RegisterType<T>(Func<T> instantiator, Action<T> disposer) => RegisterType(0, instantiator, disposer, null, null);
        public ushort RegisterType<T>(ushort explicitId, Func<T> instantiator, Action<T> disposer) => RegisterType<T>(explicitId, instantiator, disposer, null, null);
        public ushort RegisterType<T>(ushort explicitId, IPacketSerializer<T> serializer, IPacketDeserializer<T> deserializer) => RegisterType<T>(explicitId, null, null, serializer, deserializer);
        public ushort RegisterType<T>(ushort explicitId, Func<T> instantiator, Action<T> disposer, IPacketSerializer<T> serializer, IPacketDeserializer<T> deserializer)
        {
            Type tType = typeof(T);

            explicitId = GetId(tType, explicitId);


            INetworkType newRegistration = null;

            if (serializer != null && deserializer != null)
            {
                newRegistration = new DefaultSerializableNetworkType<T>(new DefaultNetworkType<T>(explicitId, instantiator, disposer), deserializer, serializer);
            }
            else
            {
                newRegistration = new DefaultNetworkType<T>(explicitId, instantiator, disposer);
            }

            registeredTypes.Add(explicitId, newRegistration);

            idRegistry.Add(tType, explicitId);

            return explicitId;
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

                return interopId?.Id ?? 0;

                if (explicitId == 0)
                {
                    throw new InvalidOperationException($"Attempted to register {type.FullName} as a network type but the class does not implement {nameof(InteropIdAttribute)}. If this is not possible use {nameof(RegisterType)} with an {nameof(explicitId)} parameter to explicitly define it.");
                }
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
    }
}
