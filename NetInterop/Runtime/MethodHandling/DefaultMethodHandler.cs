using NetInterop.Abstractions;
using NetInterop.Transport.Core.Abstractions.Packets;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace NetInterop.Runtime.MethodHandling
{
    public class DefaultMethodHandler : INetworkMethodHandler
    {
        private readonly IPointerProvider pointerProvider;
        private readonly INetTypeHandler typeHandler;
        private readonly IObjectHeap heap;
        private readonly IDictionary<INetPtr, RegisteredMethod> registeredMethods = new Dictionary<INetPtr, RegisteredMethod>();
        private readonly IDictionary<MethodInfo, INetPtr> methodPtrs = new ConcurrentDictionary<MethodInfo, INetPtr>();
        private ushort nextId = 1;

        public DefaultMethodHandler(IPointerProvider pointerProvider, INetTypeHandler typeHandler, IObjectHeap heap)
        {
            this.pointerProvider = pointerProvider ?? throw new ArgumentNullException(nameof(pointerProvider));
            this.typeHandler = typeHandler ?? throw new ArgumentNullException(nameof(typeHandler));
            this.heap = heap ?? throw new ArgumentNullException(nameof(heap));
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
            if (methodPtrs.ContainsKey(method))
            {
                return methodPtrs[method];
            }

            INetworkType declaringNetwork = null;

            // get the network type for the declaring type for the method
            if (method.IsStatic is false)
            {
                if (typeHandler.TryGetType(method.DeclaringType, out _) is false)
                {
                    throw new InvalidOperationException($"Failed to register the method {method.Name}. {method.Name} is declared as an instance method (non-static) and requires a reference of an object to be invoked, however, the declaring type {method.DeclaringType.FullName} is not registered with the {nameof(INetworkTypeHandler)}.");
                }
            }

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

            RegisteredMethod registration = new RegisteredMethod(method, returnParam, parameters.ToArray(), pointerProvider, heap);

            INetPtr ptr = pointerProvider.Create(declaringNetwork?.InteropId ?? 0, nextId++);

            methodPtrs.Add(method, ptr);

            registeredMethods.Add(ptr, registration);

            return ptr;
        }

        public bool TryGetSerializer(INetPtr ptr, out IPacketSerializer<object[]> serializer)
        {
            serializer = default;
            if (registeredMethods.ContainsKey(ptr))
            {
                serializer = registeredMethods[ptr];
                return true;
            }
            return false;
        }

        public bool TryGetDeserializer(INetPtr ptr, out IPacketDeserializer<object[]> deserializer)
        {
            deserializer = default;
            if (registeredMethods.ContainsKey(ptr))
            {
                deserializer = registeredMethods[ptr];
                return true;
            }
            return false;
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
            if (typeHandler.TryGetSerializableType(paramType, out ISerializableNetType networkType))
            {
                return new MethodParameter(info, networkType, networkType);
            }

            // if it's System.Void, then it's a return parameter and is an allowable edge case
            if (paramType == typeof(void))
            {
                return new MethodParameter(info, null, null);
            }

            throw new InvalidOperationException($"Failed to register method {method.Name} with the {nameof(INetworkMethodHandler)}. The parameter {info.Name}({paramType.FullName}) is not registered as a serializable and deserializable type with the {nameof(INetworkTypeHandler)}. Use {nameof(INetworkTypeHandler)}.Register<T>(ushort id, {nameof(IPacketSerializer)}<T> serializer, {nameof(IPacketDeserializer)}<T> deserializer) to register one.");
        }

        public void Clear()
        {
            registeredMethods.Clear();
        }
    }
}
