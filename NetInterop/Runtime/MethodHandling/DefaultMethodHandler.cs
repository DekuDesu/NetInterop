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
    public class DefaultMethodHandler : IMethodHandler
    {
        private readonly IPointerProvider pointerProvider;
        private readonly ITypeHandler typeHandler;
        private readonly IObjectHeap heap;
        private readonly IDictionary<INetPtr, RegisteredMethod> registeredMethods = new Dictionary<INetPtr, RegisteredMethod>();
        private readonly IDictionary<MethodInfo, INetPtr> methodPtrs = new ConcurrentDictionary<MethodInfo, INetPtr>();
        private ushort nextId = 1;

        public DefaultMethodHandler(IPointerProvider pointerProvider, ITypeHandler typeHandler, IObjectHeap heap)
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

        public INetPtr<TResult> Register<TResult>(Delegate method) => Register<TResult>(method.Method);

        public INetPtr Register(Delegate method) => Register(method.Method);

        public INetPtr Register(MethodInfo method)
        {
            RegisteredMethod registration = CreateRegistration(method);

            INetPtr ptr = default;

            if (method.ReturnType != typeof(void))
            {
                var genericCreateMethod = typeof(IPointerProvider)
                    .GetMethods()
                    .Where(m => m.Name == nameof(IPointerProvider.Create) && m.IsGenericMethod)
                    .FirstOrDefault();

                var contructedGeneric = genericCreateMethod.MakeGenericMethod(method.ReturnType);

                ptr = (INetPtr)contructedGeneric.Invoke(pointerProvider, 
                    parameters: new object[] {
                        registration?.DeclaringType?.TypePointer?.PtrType ?? 0, 
                        nextId++ 
                    });
            }
            else {
                ptr = pointerProvider.Create(registration?.DeclaringType?.TypePointer?.PtrType ?? 0, nextId++);
            } 

            AddRegistration(method, registration, ptr);

            return ptr;
        }

        public INetPtr<TResult> Register<TResult>(MethodInfo method)
        {
            if (methodPtrs.ContainsKey(method))
            {
                if (methodPtrs[method] is INetPtr<TResult> isDestinationType)
                {
                    return isDestinationType;
                }
                else
                {
                    throw new InvalidCastException($"Attempted to cast a method who's return type is {method.ReturnType.Name} to {typeof(TResult).Name}");
                }
            }

            RegisteredMethod registration = CreateRegistration(method);

            INetPtr<TResult> ptr = pointerProvider.Create<TResult>(registration?.DeclaringType?.TypePointer?.PtrType ?? 0, nextId++);

            AddRegistration(method, registration, ptr);

            return ptr;
        }

        private void AddRegistration(MethodInfo method, RegisteredMethod registration, INetPtr ptr)
        {
            methodPtrs.Add(method, ptr);

            registeredMethods.Add(ptr, registration);
        }

        private RegisteredMethod CreateRegistration(MethodInfo method)
        {
            IType declaringNetwork = null;

            // get the network type for the declaring type for the method
            if (method.IsStatic is false)
            {
                if (typeHandler.TryGetType(method.DeclaringType, out declaringNetwork) is false)
                {
                    throw new InvalidOperationException($"Failed to register the method {method.Name}. {method.Name} is declared as an instance method (non-static) and requires a reference of an object to be invoked, however, the declaring type {method.DeclaringType.FullName} is not registered with the {nameof(ITypeHandler)}.");
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

            return new RegisteredMethod(method, returnParam, parameters.ToArray(), pointerProvider, heap, declaringNetwork);
        }

        public bool TryGetSerializer(INetPtr methodPtr, out IPacketSerializer<object[]> serializer)
        {
            serializer = default;
            if (registeredMethods.ContainsKey(methodPtr))
            {
                serializer = registeredMethods[methodPtr];
                return true;
            }
            return false;
        }

        public bool TryGetDeserializer(INetPtr methodPtr, out IPacketDeserializer<object[]> deserializer)
        {
            deserializer = default;
            if (registeredMethods.ContainsKey(methodPtr))
            {
                deserializer = registeredMethods[methodPtr];
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
            if (typeHandler.TryGetSerializableType(paramType, out ISerializableType networkType))
            {
                return new MethodParameter(info, networkType, networkType);
            }

            // if it's System.Void, then it's a return parameter and is an allowable edge case
            if (paramType == typeof(void))
            {
                return new MethodParameter(info, null, null);
            }

            throw new InvalidOperationException($"Failed to register method {method.Name} with the {nameof(IMethodHandler)}. The parameter {info.Name}({paramType.FullName}) is not registered as a serializable and deserializable type with the {nameof(ITypeHandler)}. Use {nameof(ITypeHandler)}.Register<T>(ushort id, {nameof(IPacketSerializer)}<T> serializer, {nameof(IPacketDeserializer)}<T> deserializer) to register one.");
        }

        public void Clear()
        {
            registeredMethods.Clear();
        }

    }
}
