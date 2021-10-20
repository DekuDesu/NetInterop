using NetInterop.Abstractions;
using NetInterop.Attributes;
using NetInterop.Runtime.TypeHandling;
using NetInterop.Transport.Core.Abstractions.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace NetInterop.Runtime.Extensions
{
    public static class TypeHandlerRegistrationExtensions
    {
        public static INetPtr<T> RegisterType<T>(this ITypeHandler handler)
            => RegisterType<T>(handler, GetInteropAttributeId<T>());

        public static INetPtr<T> RegisterType<T>(this ITypeHandler handler,
            ushort interopId)
            => handler.RegisterType<T>(interopId,
                new DefaultActivator<T>(),
                new DefaultDeactivator<T>(),
                null,
                null);

        public static INetPtr<T> RegisterType<T>(this ITypeHandler handler,
            Func<T> activator)
            => RegisterType<T>(handler, GetInteropAttributeId<T>(), activator);

        public static INetPtr<T> RegisterType<T>(this ITypeHandler handler,
            ushort interopId,
            Func<T> activator)
            => handler.RegisterType<T>(interopId,
                new FuncActivator<T>(activator),
                new DefaultDeactivator<T>(),
                null,
                null);

        public static INetPtr<T> RegisterType<T>(this ITypeHandler handler,
            Func<T> activator,
            RefAction<T> disposer)
            => RegisterType<T>(handler, GetInteropAttributeId<T>(), activator, disposer);

        public static INetPtr<T> RegisterType<T>(this ITypeHandler handler,
            ushort interopId,
            Func<T> activator,
            RefAction<T> disposer)
            => handler.RegisterType<T>(interopId,
                new FuncActivator<T>(activator),
                new ActionDeactivator<T>(disposer),
                null,
                null);

        public static INetPtr<T> RegisterType<T>(this ITypeHandler handler,
            ushort interopId,
            IActivator<T> activator,
            IDeactivator<T> disposer)
            => handler.RegisterType<T>(interopId,
                activator,
                disposer,
                null,
                null);

        public static INetPtr<T> RegisterType<T>(this ITypeHandler handler,
            Func<T> activator,
            RefAction<T> disposer,
            IPacketSerializer<T> serializer)
            => RegisterType<T>(handler, GetInteropAttributeId<T>(), activator, disposer, serializer);


        public static INetPtr<T> RegisterType<T>(this ITypeHandler handler,
            ushort interopId,
            Func<T> activator,
            RefAction<T> disposer,
            IPacketSerializer<T> serializer)
            => handler.RegisterType<T>(interopId,
                new FuncActivator<T>(activator),
                new ActionDeactivator<T>(disposer),
                serializer,
                null);

        public static INetPtr<T> RegisterType<T>(this ITypeHandler handler,
            Func<T> activator,
            IPacketSerializer<T> serializer,
            IPacketDeserializer<T> deserializer)
            => handler.RegisterType<T>(
                GetInteropAttributeId<T>(),
                new FuncActivator<T>(activator),
                new DefaultDeactivator<T>(),
                serializer,
                deserializer);

        public static INetPtr<T> RegisterType<T>(this ITypeHandler handler,
            ushort interopId,
            Func<T> activator,
            IPacketSerializer<T> serializer,
            IPacketDeserializer<T> deserializer)
            => handler.RegisterType<T>(
                interopId,
                new FuncActivator<T>(activator),
                new DefaultDeactivator<T>(),
                serializer,
                deserializer);

        public static INetPtr<T> RegisterType<T>(this ITypeHandler handler,
            Func<T> activator,
            RefAction<T> disposer,
            IPacketSerializer<T> serializer,
            IPacketDeserializer<T> deserializer)
            => RegisterType<T>(handler, GetInteropAttributeId<T>(), activator, disposer, serializer, deserializer);

        public static INetPtr<T> RegisterType<T>(this ITypeHandler handler,
            ushort interopId,
            Func<T> activator,
            RefAction<T> disposer,
            IPacketSerializer<T> serializer,
            IPacketDeserializer<T> deserializer)
            => handler.RegisterType<T>(interopId,
                new FuncActivator<T>(activator),
                new ActionDeactivator<T>(disposer),
                serializer,
                deserializer);

        public static INetPtr<T> RegisterType<T>(this ITypeHandler handler,
            IPacketSerializer<T> serializer,
            IPacketDeserializer<T> deserializer)
            => RegisterType<T>(handler, GetInteropAttributeId<T>(), serializer, deserializer);

        public static INetPtr<T> RegisterType<T>(this ITypeHandler handler,
            ushort interopId,
            IPacketSerializer<T> serializer,
            IPacketDeserializer<T> deserializer)
            => handler.RegisterType<T>(interopId,
                new DefaultActivator<T>(),
                new DefaultDeactivator<T>(),
                serializer,
                deserializer);

        public static INetPtr<T> RegisterType<T, U>(this ITypeHandler handler,
            U serializer) where U : IActivator<T>, IDeactivator<T>, IPacketSerializer<T>, IPacketDeserializer<T>
            => RegisterType<T, U>(handler, GetInteropAttributeId<T>(), serializer);

        public static INetPtr<T> RegisterType<T, U>(this ITypeHandler handler) where U : IActivator<T>, IDeactivator<T>, IPacketSerializer<T>, IPacketDeserializer<T>
            => RegisterType<T, U>(handler, GetInteropAttributeId<T>(), GetInstanceOrThrow<U>());

        public static INetPtr<T> RegisterType<T, U>(this ITypeHandler handler,
            ushort interopId) where U : IActivator<T>, IDeactivator<T>, IPacketSerializer<T>, IPacketDeserializer<T>
        {
            var serializer = GetInstanceOrThrow<U>();
            return handler.RegisterType<T>(interopId,
                serializer,
                serializer,
                serializer,
                serializer);
        }

        public static INetPtr<T> RegisterType<T, U>(this ITypeHandler handler,
            ushort interopId,
            U serializer) where U : IActivator<T>, IDeactivator<T>, IPacketSerializer<T>, IPacketDeserializer<T>
            => handler.RegisterType<T>(interopId,
                serializer,
                serializer,
                serializer,
                serializer);

        private static ushort GetInteropAttributeId<T>()
        {
            InteropIdAttribute interopId = typeof(T).GetCustomAttribute<InteropIdAttribute>();

            if (interopId is null)
            {
                throw new InvalidOperationException($"Attempted to register {typeof(T).FullName} as a network type but the class does not implement {nameof(InteropIdAttribute)}. If this is not possible use {nameof(RegisterType)} with an {nameof(interopId)} parameter to explicitly define it.");
            }

            return interopId.Id;
        }

        private static T GetInstanceOrThrow<T>()
        {
            Type type = typeof(T);

            // check to see if it has a public parameterless constructor
            var parameterlessConstructor = type.GetConstructor(BindingFlags.Public | BindingFlags.Instance, null, Array.Empty<Type>(), null);

            if (parameterlessConstructor != null)
            {
                return (T)parameterlessConstructor.Invoke(Array.Empty<object>());
            }

            // if no parameterless constructor exists check to see if the object is a singleton
            PropertyInfo[] staticProperties = type.GetProperties(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);

            var propertiesMatchingType = staticProperties.Where(x => x.PropertyType == type);

            // we should prioritize properties that follow the singleton pattern by name matching
            // if we cant find one we should then accept any public static property with matching type, but warn the user

            const string error = "Failed create an instance of the activator/serializer: {0} since it has no public parameterless constructor and no public/private static property with the type of {0}.";

            if (propertiesMatchingType is null || propertiesMatchingType.Count() is 0)
            {
                throw new NotSupportedException(string.Format(error, type.FullName));
            }

            // magic word but this should be prioritized
            var instanceProperty = propertiesMatchingType.Where(x => x.Name.ToLowerInvariant() == "instance").FirstOrDefault();

            if (instanceProperty != null)
            {
                return (T)instanceProperty.GetValue(null);
            }

            // if no property matches the magic word just return any one of them
            return (T)propertiesMatchingType.First().GetValue(null);
        }
    }
}
