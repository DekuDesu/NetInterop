using NetInterop.Abstractions;
using NetInterop.Attributes;
using NetInterop.Runtime.TypeHandling;
using NetInterop.Transport.Core.Abstractions.Packets;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NetInterop.Runtime.Extensions
{
    public static class TypeHandlerRegistrationExtensions
    {
        public static INetPtr<T> RegisterType<T>(this INetTypeHandler handler)
            => RegisterType<T>(handler, GetInteropAttributeId<T>());

        public static INetPtr<T> RegisterType<T>(this INetTypeHandler handler,
            ushort interopId)
            => handler.RegisterType<T>(interopId,
                new DefaultActivator<T>(),
                new DefaultDeactivator<T>(),
                null,
                null);

        public static INetPtr<T> RegisterType<T>(this INetTypeHandler handler,
            Func<T> activator)
            => RegisterType<T>(handler, GetInteropAttributeId<T>(), activator);

        public static INetPtr<T> RegisterType<T>(this INetTypeHandler handler,
            ushort interopId,
            Func<T> activator)
            => handler.RegisterType<T>(interopId,
                new FuncActivator<T>(activator),
                new DefaultDeactivator<T>(),
                null,
                null);

        public static INetPtr<T> RegisterType<T>(this INetTypeHandler handler,
            Func<T> activator,
            Action<T> disposer)
            => RegisterType<T>(handler, GetInteropAttributeId<T>(), activator, disposer);

        public static INetPtr<T> RegisterType<T>(this INetTypeHandler handler,
            ushort interopId,
            Func<T> activator,
            Action<T> disposer)
            => handler.RegisterType<T>(interopId,
                new FuncActivator<T>(activator),
                new ActionDeactivator<T>(disposer),
                null,
                null);

        public static INetPtr<T> RegisterType<T>(this INetTypeHandler handler,
            Func<T> activator,
            Action<T> disposer,
            IPacketSerializer<T> serializer)
            => RegisterType<T>(handler, GetInteropAttributeId<T>(), activator, disposer, serializer);


        public static INetPtr<T> RegisterType<T>(this INetTypeHandler handler,
            ushort interopId,
            Func<T> activator,
            Action<T> disposer,
            IPacketSerializer<T> serializer)
            => handler.RegisterType<T>(interopId,
                new FuncActivator<T>(activator),
                new ActionDeactivator<T>(disposer),
                serializer,
                null);

        public static INetPtr<T> RegisterType<T>(this INetTypeHandler handler,
            Func<T> activator,
            IPacketSerializer<T> serializer,
            IPacketDeserializer<T> deserializer)
            => handler.RegisterType<T>(
                GetInteropAttributeId<T>(),
                new FuncActivator<T>(activator),
                new DefaultDeactivator<T>(),
                serializer,
                deserializer);

        public static INetPtr<T> RegisterType<T>(this INetTypeHandler handler,
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

        public static INetPtr<T> RegisterType<T>(this INetTypeHandler handler,
            Func<T> activator,
            Action<T> disposer,
            IPacketSerializer<T> serializer,
            IPacketDeserializer<T> deserializer)
            => RegisterType<T>(handler, GetInteropAttributeId<T>(), activator, disposer, serializer, deserializer);

        public static INetPtr<T> RegisterType<T>(this INetTypeHandler handler,
            ushort interopId,
            Func<T> activator,
            Action<T> disposer,
            IPacketSerializer<T> serializer,
            IPacketDeserializer<T> deserializer)
            => handler.RegisterType<T>(interopId,
                new FuncActivator<T>(activator),
                new ActionDeactivator<T>(disposer),
                serializer,
                deserializer);

        public static INetPtr<T> RegisterType<T>(this INetTypeHandler handler,
            IPacketSerializer<T> serializer,
            IPacketDeserializer<T> deserializer)
            => RegisterType<T>(handler, GetInteropAttributeId<T>(), serializer, deserializer);

        public static INetPtr<T> RegisterType<T>(this INetTypeHandler handler,
            ushort interopId,
            IPacketSerializer<T> serializer,
            IPacketDeserializer<T> deserializer)
            => handler.RegisterType<T>(interopId,
                new DefaultActivator<T>(),
                new DefaultDeactivator<T>(),
                serializer,
                deserializer);

        private static ushort GetInteropAttributeId<T>()
        {
            InteropIdAttribute interopId = typeof(T).GetCustomAttribute<InteropIdAttribute>();

            if (interopId is null)
            {
                throw new InvalidOperationException($"Attempted to register {typeof(T).FullName} as a network type but the class does not implement {nameof(InteropIdAttribute)}. If this is not possible use {nameof(RegisterType)} with an {nameof(interopId)} parameter to explicitly define it.");
            }

            return interopId.Id;
        }
    }
}
