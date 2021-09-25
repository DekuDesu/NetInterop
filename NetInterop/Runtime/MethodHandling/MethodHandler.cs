using NetInterop.Transport.Core.Abstractions.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace NetInterop.Runtime.MethodHandling
{
    public class DefaultMethodHandler : INetworkMethodHandler
    {
        private readonly IPointerProvider pointerProvider;
        private readonly INetworkTypeHandler typeHandler;
        private readonly IDictionary<INetPtr, RegisteredMethod> registeredMethods = new Dictionary<INetPtr, RegisteredMethod>();
        private ushort nextId = 1;

        public DefaultMethodHandler(IPointerProvider pointerProvider, INetworkTypeHandler typeHandler)
        {
            this.pointerProvider = pointerProvider ?? throw new ArgumentNullException(nameof(pointerProvider));
            this.typeHandler = typeHandler ?? throw new ArgumentNullException(nameof(typeHandler));
        }

        public void Invoke(INetPtr methodPtr, IPacket packet) => Invoke(methodPtr, packet, null);

        public void Invoke(INetPtr methodPtr, IPacket packet, IPacket packetBuilder)
        {
            if (registeredMethods.ContainsKey(methodPtr))
            {
                registeredMethods[methodPtr].Invoke(packet, packetBuilder);
            }
        }

        public INetPtr Register(Delegate method) => Register(method.Method);

        public INetPtr Register(MethodInfo method)
        {
            // iterate and generate parameters for the method
            // making sure that every type the method accepts as a parameter is registerd with the type handler
            ParameterInfo[] parameterInfos = method.GetParameters();

            MethodParameter[] parameters = new MethodParameter[parameterInfos.Length];

            for (int i = 0; i < parameterInfos.Length; i++)
            {
                ParameterInfo item = parameterInfos[i];
                parameters[i] = EnsureRegistered(item, method);
            }

            // get the return type and ensure that is as well is registered
            MethodParameter returnParam = EnsureRegistered(method.ReturnParameter, method);

            INetworkType declaringNetwork = null;

            // get the network type for the declaring type for the method
            if (method.IsStatic is false)
            {
                if (typeHandler.TryGetTypePtr(method.DeclaringType, out INetPtr declaringPtr) is false
                    || typeHandler.TryGetAmbiguousType(declaringPtr, out declaringNetwork) is false)
                {
                    throw new InvalidOperationException($"Failed to register the method {method.Name}. {method.Name} is declared as an instance method (non-static) and requires a reference of an object to be invoked, however, the declaring type {method.DeclaringType.FullName} is not registered with the {nameof(INetworkTypeHandler)}.");
                }
            }

            RegisteredMethod registration = new RegisteredMethod(method, returnParam, parameters.ToArray(), pointerProvider, declaringNetwork);

            INetPtr ptr = pointerProvider.Create(0, nextId++);

            registeredMethods.Add(ptr, registration);

            return ptr;
        }

        private MethodParameter EnsureRegistered(ParameterInfo info, MethodInfo method)
        {
            Type paramType = info.ParameterType;

            // make sure the type is registered in the type handler
            if (paramType.IsArray)
            {
                // if it's an array get it's generic type, and ensure the generic type is registered instead
                paramType = paramType.GetGenericArguments().FirstOrDefault();
            }

            // make sure the type is registered as serializable
            if (typeHandler.TryGetTypePtr(paramType, out INetPtr typePtr))
            {
                if (typeHandler.TryGetAmbiguousSerializableType(typePtr, out var networkType))
                {
                    return new MethodParameter(info, networkType, networkType);
                }
            }

            // if it's System.Void, then it's a return parameter and is an allowable edge case
            if (paramType == typeof(void))
            {
                return new MethodParameter(info, null, null);
            }

            throw new InvalidOperationException($"Failed to register method {method.Name} with the {nameof(INetworkMethodHandler)}. The parameter {info.Name}({paramType.FullName}) is not registered as a serializable and deserializable type with the {nameof(INetworkTypeHandler)}. Use {nameof(INetworkTypeHandler)}.Register<T>(ushort id, {nameof(IPacketSerializer)}<T> serializer, {nameof(IPacketDeserializer)}<T> deserializer) to register one.");
        }
    }
}
