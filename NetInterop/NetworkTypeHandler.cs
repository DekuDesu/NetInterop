using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using NetInterop.Attributes;

namespace NetInterop
{
    public class NetworkTypeHandler : INetworkTypeHandler
    {
        private readonly IDictionary<ushort, INetworkType> registeredTypes = new Dictionary<ushort, INetworkType>();
        private readonly IDictionary<Type, ushort> idRegistry = new Dictionary<Type, ushort>();

        public void RegisterUnmanagedTypes()
        {
            this.RegisterType<int>((ushort)Type.GetTypeCode(typeof(int)));
        }

        public ushort RegisterType(Type type, ushort explicitId, object instantiator)
        {
            explicitId = GetId(type, explicitId);

            // create the network register type
            var constructedGeneric = typeof(RegisteredNetworkType<>).MakeGenericType(new Type[] { type });

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

        public INetworkType<T> GetNetworkType<T>()
        {
            ushort id = idRegistry[typeof(T)];

            if (registeredTypes.ContainsKey(id))
            {
                INetworkType possiblyRegisteredType = registeredTypes[id];

                if (possiblyRegisteredType is INetworkType<T> isRegisteredType)
                {
                    return isRegisteredType;
                }
            }

            return null;
        }

        public INetworkType<T> GetNetworkType<T>(INetPtr<T> id)
        {
            if (registeredTypes.ContainsKey(id.PtrType))
            {
                INetworkType possiblyRegisteredType = registeredTypes[id.PtrType];

                if (possiblyRegisteredType is INetworkType<T> isRegisteredType)
                {
                    return isRegisteredType;
                }
            }
            return null;
        }

        public INetworkType GetAmbiguousNetworkType(INetPtr id)
        {
            if (registeredTypes.ContainsKey(id.PtrType))
            {
                return registeredTypes[id.PtrType];
            }
            return null;
        }


        public ushort RegisterType<T>(ushort explicitId) => RegisterType<T>(explicitId, null, null);
        public ushort RegisterType<T>(Func<T> instantiator) => RegisterType<T>(0, instantiator, null);
        public ushort RegisterType<T>(Action<T> disposer) => RegisterType<T>(0, null, disposer);
        public ushort RegisterType<T>(Func<T> instantiator, Action<T> disposer) => RegisterType(0, instantiator, disposer);
        public ushort RegisterType<T>(ushort explicitId, Func<T> instantiator, Action<T> disposer)
        {
            Type tType = typeof(T);

            explicitId = GetId(tType, explicitId);

            var newRegistration = new RegisteredNetworkType<T>(explicitId, instantiator, disposer);

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


    }
}
