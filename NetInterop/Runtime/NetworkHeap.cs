using NetInterop.Abstractions;
using NetInterop.Transport.Core.Abstractions.Packets;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetInterop
{
    public class NetworkHeap : INetworkHeap
    {
        private readonly IMethodHandler methodHandler;
        private readonly ITypeHandler typeHandler;
        private readonly IPacketSender sender;
        private readonly IDelegateHandler<bool, IPacket> callbackDelegateHandler;
        public NetworkHeap(ITypeHandler typeHandler, IPacketSender sender, IDelegateHandler<bool, IPacket> callbackDelegateHandler, IMethodHandler methodHandler)
        {
            this.typeHandler = typeHandler ?? throw new ArgumentNullException(nameof(typeHandler));
            this.sender = sender ?? throw new ArgumentNullException(nameof(sender));
            this.callbackDelegateHandler = callbackDelegateHandler ?? throw new ArgumentNullException(nameof(callbackDelegateHandler));
            this.methodHandler = methodHandler ?? throw new ArgumentNullException(nameof(methodHandler));
        }

        public IPointerProvider PointerProvider { get; set; } = new DefaultPointerProvider();

        public void Create<T>(Action<INetPtr<T>> callback)
        {
            if (typeHandler.TryGetType<T>(out IType<T> type))
            {
                sender.Send(new PointerOperationPacket(PointerOperations.Alloc, new CallbackPacket(
                    (goodResponse, packet) =>
                    {
                        if (goodResponse)
                        {
                            callback(PointerProvider.Deserialize(packet).As<T>());
                        }
                    },
                    type.TypePointer, callbackDelegateHandler)));
                return;
            }
        }

        public void Create(INetPtr typePtr, Action<INetPtr> callback)
        {
            sender.Send(new PointerOperationPacket(PointerOperations.Alloc, new CallbackPacket(
                    (goodResponse, packet) =>
                    {
                        if (goodResponse)
                        {
                            callback(PointerProvider.Deserialize(packet));
                        }
                    },
                    typePtr, callbackDelegateHandler)));
            return;
        }

        public void Destroy<T>(INetPtr<T> ptr)
        {
            sender.Send(new PointerOperationPacket(PointerOperations.Free, new CallbackPacket(null, ptr, callbackDelegateHandler)));
        }

        public void Destroy(INetPtr ptr)
        {
            sender.Send(new PointerOperationPacket(PointerOperations.Free, new CallbackPacket(null, ptr, callbackDelegateHandler)));
        }

        public void Get<T>(INetPtr<T> ptr, Action<T> callback)
        {
            if (typeHandler.TryGetSerializableType<T>(ptr, out var serializer))
            {
                sender.Send(new PointerOperationPacket(PointerOperations.Get, new CallbackPacket(
                    (goodResponse, packet) =>
                    {
                        if (goodResponse)
                        {
                            callback(serializer.Deserialize(packet));
                            return;
                        }

                        callback(default(T));
                    },
                    ptr,
                    callbackDelegateHandler
                )));
                return;
            }
            throw new InvalidOperationException($"Failed to get a remote instance of type {typeof(T).FullName}, it was not registered in the {nameof(typeHandler)}, use INetworkTypeHandler.Register<T>(id) to register the type as a network type ensure a valid IPacketDeserializer is registered to properly deserialize the type.");
        }

        public void Get(INetPtr ptr, Action<object> callback)
        {
            if (typeHandler.TryGetSerializableType(ptr, out var serializer))
            {
                sender.Send(new PointerOperationPacket(PointerOperations.Get, new CallbackPacket(
                    (goodResponse, packet) =>
                    {
                        if (goodResponse)
                        {
                            callback(serializer.AmbiguousDeserialize(packet));
                            return;
                        }

                        callback(null);
                    },
                    ptr,
                    callbackDelegateHandler
                )));
                return;
            }
            throw new InvalidOperationException($"Failed to get a remote instance of net interop pointer {ptr}, it was not registered in the {nameof(typeHandler)}, use INetworkTypeHandler.Register<T>(id) to register the type as a network type ensure a valid IPacketDeserializer is registered to properly deserialize the type.");
        }

        public void Set<T>(INetPtr<T> ptr, T value)
        {
            if (typeHandler.TryGetSerializableType<T>(ptr, out ISerializableType<T> serializer))
            {
                sender.Send(new PointerOperationPacket(PointerOperations.Set, new CallbackPacket(
                    null,
                    new SetPointerPacket<T>(ptr, value, serializer),
                    callbackDelegateHandler
                )));
                return;
            }
            throw new InvalidOperationException($"Failed to get a remote instance of type {typeof(T).FullName}, it was not registered in the {nameof(typeHandler)}, use INetworkTypeHandler.Register<T>(id) to register the type as a network type ensure a valid IPacketDeserializer is registered to properly deserialize the type.");
        }

        public void Set(INetPtr ptr, object value)
        {
            if (typeHandler.TryGetSerializableType(ptr, out var serializer))
            {
                sender.Send(new PointerOperationPacket(PointerOperations.Set, new CallbackPacket(
                    null,
                    new SetAmbiguousPointerPacket(ptr, value, serializer),
                    callbackDelegateHandler
                )));
                return;
            }
            throw new InvalidOperationException($"Failed to get a remote instance of net interop pointer {ptr}, it was not registered in the {nameof(typeHandler)}, use INetworkTypeHandler.Register<T>(id) to register the type as a network type ensure a valid IPacketDeserializer is registered to properly deserialize the type.");
        }

        public void InvokeStatic(INetPtr methodPtr)
        {
            sender.Send(new PointerOperationPacket(PointerOperations.Invoke, new CallbackPacket(null, methodPtr, callbackDelegateHandler)));
        }

        public void InvokeStatic<T>(INetPtr methodPtr, Action<T> callback)
        {
            if (typeHandler.TryGetSerializableType<T>(out var serializer) is false)
            {
                throw GenerateMissingReturnTypeSerializerException(typeof(T).FullName);
            }

            sender.Send(new PointerOperationPacket(PointerOperations.Invoke, new CallbackPacket(
                (goodResponse, packet) =>
                {
                    if (goodResponse)
                    {
                        callback(serializer.Deserialize(packet));
                        return;
                    }

                    callback(default(T));
                },
                methodPtr,
                callbackDelegateHandler
            )));
        }

        public void InvokeStatic(INetPtr methodPtr, params object[] parameters)
        {
            if (methodHandler.TryGetSerializer(methodPtr, out var parameterSerializer) is false)
            {
                throw GenerateMissingMethodSerializerException(methodPtr.ToString());
            }

            sender.Send(new PointerOperationPacket(PointerOperations.Invoke, new CallbackPacket(
                null,
                new InvokePointerWithParametersPacket(methodPtr, parameterSerializer, parameters),
                callbackDelegateHandler
            )));

        }

        public void InvokeStatic<T>(INetPtr methodPtr, Action<T> callback, params object[] parameters)
        {
            if (methodHandler.TryGetSerializer(methodPtr, out var serializer) is false)
            {
                throw GenerateMissingMethodSerializerException(methodPtr.ToString());
            }

            if (typeHandler.TryGetSerializableType<T>(out var resultSerializer) is false)
            {
                throw GenerateMissingReturnTypeSerializerException(typeof(T).FullName);
            }

            sender.Send(new PointerOperationPacket(PointerOperations.Invoke, new CallbackPacket(
                (goodResponse, packet) =>
                {
                    if (goodResponse)
                    {
                        callback(resultSerializer.Deserialize(packet));
                        return;
                    }

                    callback(default(T));
                },
                new InvokePointerWithParametersPacket(methodPtr, serializer, parameters),
                callbackDelegateHandler
            )));
        }

        public void Invoke<T>(INetPtr methodPtr, Action<T> callback, params object[] parameters)
        {
            if (methodHandler.TryGetSerializer(methodPtr, out var parametersSerializer) is false)
            {
                throw GenerateMissingMethodSerializerException(methodPtr.ToString());
            }

            if (typeHandler.TryGetSerializableType<T>(out var resultSerializer) is false)
            {
                throw GenerateMissingReturnTypeSerializerException(typeof(T).FullName);
            }

            sender.Send(new PointerOperationPacket(PointerOperations.Invoke, new CallbackPacket(
                    (goodResponse, packet) =>
                    {
                        if (goodResponse)
                        {
                            callback(resultSerializer.Deserialize(packet));
                            return;
                        }

                        callback(default(T));
                    },
                    new InvokePointerWithParametersPacket(methodPtr, parametersSerializer, parameters),
                    callbackDelegateHandler
                )));

        }

        public void Invoke<T>(INetPtr methodPtr, INetPtr<T> instancePtr) => Invoke(methodPtr, (INetPtr)instancePtr);

        public void Invoke(INetPtr methodPtr, INetPtr instancePtr)
        {
            if (methodPtr.PtrType != instancePtr.PtrType)
            {
                throw new InvalidCastException($"Can't invoke method {methodPtr} on instance {instancePtr} since the instance type is not the declaring type that defines the method. If the method is static use InvokeStatic(methodPtr) instead.");
            }

            sender.Send(new PointerOperationPacket(PointerOperations.Invoke, new CallbackPacket(
                null,
                new InvokePointerWithInstance(methodPtr, instancePtr),
                callbackDelegateHandler
            )));
        }

        public void Invoke<T>(INetPtr methodPtr, INetPtr<T> instancePtr, Action<T> callback)
        {
            if (methodPtr.PtrType != instancePtr.PtrType)
            {
                throw GenerateMethodDeclaringTypeMismatchException(methodPtr.ToString(), instancePtr.ToString());
            }

            if (typeHandler.TryGetSerializableType<T>(out var resultSerializer) is false)
            {
                throw GenerateMissingReturnTypeSerializerException(typeof(T).FullName);
            }

            sender.Send(new PointerOperationPacket(PointerOperations.Invoke, new CallbackPacket(
                (goodResponse, packet) =>
                {
                    if (goodResponse)
                    {
                        callback(resultSerializer.Deserialize(packet));
                        return;
                    }

                    callback(default(T));
                },
                new InvokePointerWithInstance(methodPtr, instancePtr),
                callbackDelegateHandler
            )));
        }

        public void Invoke<T>(INetPtr methodPtr, INetPtr<T> instancePtr, params object[] parameters)
        {
            if (methodPtr.PtrType != instancePtr.PtrType)
            {
                throw GenerateMethodDeclaringTypeMismatchException(methodPtr.ToString(), instancePtr.ToString());
            }

            if (methodHandler.TryGetSerializer(methodPtr, out var serializer) is false)
            {
                throw GenerateMissingMethodSerializerException(methodPtr.ToString());
            }

            sender.Send(new PointerOperationPacket(PointerOperations.Invoke, new CallbackPacket(
                null,
                new InvokePointerWithInstanceAndParametersPacket(methodPtr, instancePtr, serializer, parameters),
                callbackDelegateHandler
            )));
        }

        public void Invoke<T>(INetPtr methodPtr, INetPtr<T> instancePtr, Action<T> callback, params object[] parameters)
        {
            if (methodPtr.PtrType != instancePtr.PtrType)
            {
                throw GenerateMethodDeclaringTypeMismatchException(methodPtr.ToString(), instancePtr.ToString());
            }

            if (methodHandler.TryGetSerializer(methodPtr, out var serializer) is false)
            {
                throw GenerateMissingMethodSerializerException(methodPtr.ToString());
            }

            if (typeHandler.TryGetSerializableType<T>(out var resultSerializer) is false)
            {
                throw GenerateMissingReturnTypeSerializerException(typeof(T).FullName);
            }

            sender.Send(new PointerOperationPacket(PointerOperations.Invoke, new CallbackPacket(
                (goodResponse, packet) =>
                {
                    if (goodResponse)
                    {
                        callback(resultSerializer.Deserialize(packet));
                        return;
                    }

                    callback(default(T));
                },
                new InvokePointerWithInstanceAndParametersPacket(methodPtr, instancePtr, serializer, parameters),
                callbackDelegateHandler
            )));
        }

        private Exception GenerateMissingMethodSerializerException(string methodPtr)
        {
            return new MissingMemberException($"Missing serializer and/or deserializer. The method {methodPtr} is either not registered, or is missing serializers and/or deserializers for some of its parameters and they were not found within the {nameof(ITypeHandler)}. Use {nameof(IMethodHandler.Register)} to register a {nameof(IPacketSerializer)} and {nameof(IPacketDeserializer)} for {methodPtr}.");
        }

        private Exception GenerateMissingReturnTypeSerializerException(string typeName)
        {
            return new MissingMemberException($"Missing serializer and/or deserializer. The type {typeName} was not found within the {typeHandler} with a registered serializer and deserializer. Use {nameof(ITypeHandler.RegisterType)} to register a {nameof(IPacketSerializer)} and {nameof(IPacketDeserializer)} for {typeName}.");
        }

        private Exception GenerateMethodDeclaringTypeMismatchException(string methodPtr, string instancePtr)
        {
            return new InvalidCastException($"Can't invoke method {methodPtr} on instance {instancePtr} since the instance type is not the declaring type that defines the method. If the method is static use InvokeStatic(methodPtr) instead.");
        }
    }
}
