using NetInterop.Abstractions;
using NetInterop.Transport.Core.Abstractions.Packets;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NetInterop.Runtime
{
    public class RemoteHeap : IRemoteHeap
    {
        private readonly IPointerProvider pointerProvider;
        private readonly IPacketSender sender;
        private readonly ITypeHandler typeHandler;
        private readonly IMethodHandler methodHandler;
        private readonly IDelegateHandler<bool, IPacket> callbackHandler;

        public RemoteHeap(IPointerProvider pointerProvider, IPacketSender sender, ITypeHandler typeHandler, IMethodHandler methodHandler, IDelegateHandler<bool, IPacket> callbackHandler)
        {
            this.pointerProvider = pointerProvider ?? throw new ArgumentNullException(nameof(pointerProvider));
            this.sender = sender ?? throw new ArgumentNullException(nameof(sender));
            this.typeHandler = typeHandler ?? throw new ArgumentNullException(nameof(typeHandler));
            this.methodHandler = methodHandler ?? throw new ArgumentNullException(nameof(methodHandler));
            this.callbackHandler = callbackHandler ?? throw new ArgumentNullException(nameof(callbackHandler));
        }

        #region Create
        public Task<INetPtr<T>> Create<T>(INetPtr<T> typePointer) => Create<T>();

        public Task<INetPtr<T>> Create<T>()
        {
            var taskSource = new TaskCompletionSource<INetPtr<T>>();

            void CreateCallback(bool success, IPacket response)
            {
                try
                {
                    if (success)
                    {
                        taskSource.SetResult(pointerProvider.Deserialize(response).As<T>());
                    }
                    else
                    {
                        taskSource.SetResult(null);
                    }
                }
                catch (Exception e)
                {
                    taskSource.SetException(e);
                }
            }

            // try to get the pointer associated with the type of T
            if (typeHandler.TryGetType<T>(out IType<T> type) is false) throw MissingTypeException(typeof(T).FullName);

            // create and send the packet
            var packet = new PointerOperationPacket(PointerOperations.Alloc,
                new CallbackPacket(CreateCallback, type.TypePointer, callbackHandler)
            );

            sender.Send(packet);

            return taskSource.Task;
        }

        public Task<INetPtr> Create(INetPtr typePointer)
        {
            var taskSource = new TaskCompletionSource<INetPtr>();

            void CreateCallback(bool success, IPacket response)
            {
                try
                {
                    if (success)
                    {
                        taskSource.SetResult(pointerProvider.Deserialize(response));
                    }
                    else
                    {
                        taskSource.SetResult(null);
                    }
                }
                catch (Exception e)
                {
                    taskSource.SetException(e);
                }
            }

            // try to get the pointer associated with the type of T
            if (typeHandler.TryGetType(typePointer, out IType type) is false) throw MissingTypeException(typePointer.ToString());

            // create and send the packet
            var packet = new PointerOperationPacket(PointerOperations.Alloc,
                new CallbackPacket(CreateCallback, type.TypePointer, callbackHandler)
            );

            sender.Send(packet);

            return taskSource.Task;
        }
        #endregion Create

        #region Destroy
        public Task<bool> Destroy<T>(INetPtr<T> instancePointer) => Destroy((INetPtr)instancePointer);

        public Task<bool> Destroy(INetPtr instancePointer)
        {
            var taskSource = new TaskCompletionSource<bool>();

            void DestroyCallback(bool success, IPacket response)
            {
                taskSource.SetResult(success);
            }

            var packet = new PointerOperationPacket(PointerOperations.Free,
                new CallbackPacket(DestroyCallback, instancePointer, callbackHandler)
            );

            sender.Send(packet);

            return taskSource.Task;
        }
        #endregion Destroy

        #region Get
        public Task<T> Get<T>(INetPtr<T> instancePointer)
        {
            // make sure we have all the serializers available to us before we send any packets
            if (typeHandler.TryGetSerializableType<T>(out ISerializableType<T> serializer) is false) throw MissingSerializerException(typeof(T).FullName);

            var taskSource = new TaskCompletionSource<T>();

            void GetCallback(bool sucess, IPacket response)
            {
                try
                {
                    if (sucess)
                    {
                        taskSource.SetResult(serializer.Deserialize(response));
                    }
                    else
                    {
                        taskSource.SetResult(default);
                    }
                }
                catch (Exception e)
                {
                    taskSource.SetException(e);
                }
            }

            var packet = new PointerOperationPacket(PointerOperations.Get,
                new CallbackPacket(GetCallback, instancePointer, callbackHandler)
            );

            sender.Send(packet);

            return taskSource.Task;
        }

        public Task<object> Get(INetPtr instancePointer)
        {
            // make sure we have all the serializers available to us before we send any packets
            if (typeHandler.TryGetSerializableType(instancePointer, out ISerializableType serializer) is false) throw MissingSerializerException(instancePointer.PtrType.ToString());

            var taskSource = new TaskCompletionSource<object>();

            void GetCallback(bool sucess, IPacket response)
            {
                try
                {
                    if (sucess)
                    {
                        taskSource.SetResult(serializer.AmbiguousDeserialize(response));
                    }
                    else
                    {
                        taskSource.SetResult(default);
                    }
                }
                catch (Exception e)
                {
                    taskSource.SetException(e);
                }
            }

            var packet = new PointerOperationPacket(PointerOperations.Get,
                new CallbackPacket(GetCallback, instancePointer, callbackHandler)
            );

            sender.Send(packet);

            return taskSource.Task;
        }
        #endregion Get

        #region Set
        public Task<bool> Set<T>(INetPtr<T> instancePointer, T value)
        {
            // make sure we have all the serializers available to us before we send any packets
            if (typeHandler.TryGetSerializableType<T>(out ISerializableType<T> serializer) is false) throw MissingSerializerException(typeof(T).FullName);

            var taskSource = new TaskCompletionSource<bool>();

            void GetCallback(bool sucess, IPacket response)
            {
                taskSource.SetResult(sucess);
            }

            var packet = new PointerOperationPacket(PointerOperations.Set,
                new CallbackPacket(GetCallback, new SetPointerPacket<T>(instancePointer, value, serializer), callbackHandler)
            );

            sender.Send(packet);

            return taskSource.Task;
        }

        public Task<bool> Set(INetPtr instancePointer, object value)
        {
            // make sure we have all the serializers available to us before we send any packets
            if (typeHandler.TryGetSerializableType(instancePointer, out ISerializableType serializer) is false) throw MissingSerializerException(instancePointer.PtrType.ToString());

            var taskSource = new TaskCompletionSource<bool>();

            void GetCallback(bool sucess, IPacket response)
            {
                taskSource.SetResult(sucess);
            }

            var packet = new PointerOperationPacket(PointerOperations.Set,
                new CallbackPacket(GetCallback, new SetAmbiguousPointerPacket(instancePointer, value, serializer), callbackHandler)
            );

            sender.Send(packet);

            return taskSource.Task;
        }
        #endregion Set

        #region Invoke
        public Task<bool> Invoke(INetPtr methodPointer, INetPtr instancePointer)
        {
            var taskSource = new TaskCompletionSource<bool>();

            void InvokeCallback(bool success, IPacket response)
            {
                taskSource.SetResult(success);
            }

            var packet = new PointerOperationPacket(PointerOperations.Invoke,
                new CallbackPacket(InvokeCallback, new InvokePointerWithInstance(methodPointer, instancePointer), callbackHandler)
            );

            sender.Send(packet);

            return taskSource.Task;
        }

        public Task<T> Invoke<T>(INetPtr<T> methodPointer, INetPtr instancePointer)
        {
            // we should make sure we can deserialize the result we get
            if (typeHandler.TryGetSerializableType<T>(out ISerializableType<T> serializer) is false) throw MissingSerializerException(typeof(T).FullName);

            var taskSource = new TaskCompletionSource<T>();

            void InvokeCallback(bool success, IPacket response)
            {
                try
                {
                    if (success)
                    {
                        taskSource.SetResult(serializer.Deserialize(response));
                    }
                    else
                    {
                        taskSource.SetResult(default);
                    }
                }
                catch (Exception e)
                {
                    taskSource.SetException(e);
                }
            }

            var packet = new PointerOperationPacket(PointerOperations.Invoke,
                new CallbackPacket(InvokeCallback, new InvokePointerWithInstance(methodPointer, instancePointer), callbackHandler)
            );

            sender.Send(packet);

            return taskSource.Task;
        }

        public Task<bool> Invoke(INetPtr methodPointer, INetPtr instancePointer, params object[] parameters)
        {
            // we should make sure we can deserialize the result we get
            if (methodHandler.TryGetSerializer(methodPointer, out IPacketSerializer<object[]> serializer) is false) throw MissingParameterSerializer(methodPointer.ToString());

            var taskSource = new TaskCompletionSource<bool>();

            void InvokeCallback(bool success, IPacket response)
            {
                taskSource.SetResult(success);
            }

            var packet = new PointerOperationPacket(PointerOperations.Invoke,
                wrappedPacket: new CallbackPacket(
                    callback: InvokeCallback,
                    packet: new InvokePointerWithInstanceAndParametersPacket(methodPointer, instancePointer, serializer, parameters),
                    packetCallbackHandler: callbackHandler
                )
            );

            sender.Send(packet);

            return taskSource.Task;
        }

        public Task<T> Invoke<T>(INetPtr<T> methodPointer, INetPtr instancePointer, params object[] parameters)
        {
            // we should make sure we can deserialize the result we get
            if (methodHandler.TryGetSerializer(methodPointer, out IPacketSerializer<object[]> serializer) is false) throw MissingParameterSerializer(methodPointer.ToString());
            // we should make sure we can deserialize the result we get
            if (typeHandler.TryGetSerializableType<T>(out ISerializableType<T> deserializer) is false) throw MissingSerializerException(typeof(T).FullName);

            var taskSource = new TaskCompletionSource<T>();

            void InvokeCallback(bool success, IPacket response)
            {
                try
                {
                    if (success)
                    {
                        taskSource.SetResult(deserializer.Deserialize(response));
                    }
                    else
                    {
                        taskSource.SetResult(default);
                    }
                }
                catch (Exception e)
                {
                    taskSource.SetException(e);
                }
            }

            var packet = new PointerOperationPacket(PointerOperations.Invoke,
                wrappedPacket: new CallbackPacket(
                    callback: InvokeCallback,
                    packet: new InvokePointerWithInstanceAndParametersPacket(methodPointer, instancePointer, serializer, parameters),
                    packetCallbackHandler: callbackHandler
                )
            );

            sender.Send(packet);

            return taskSource.Task;
        }

        public Task<bool> InvokeStatic(INetPtr methodPointer)
        {
            var taskSource = new TaskCompletionSource<bool>();

            void InvokeCallback(bool success, IPacket response)
            {
                taskSource.SetResult(success);
            }

            var packet = new PointerOperationPacket(PointerOperations.Invoke,
                wrappedPacket: new CallbackPacket(
                    callback: InvokeCallback,
                    packet: methodPointer,
                    packetCallbackHandler: callbackHandler
                )
            );

            sender.Send(packet);

            return taskSource.Task;
        }

        public Task<bool> InvokeStatic(INetPtr methodPointer, params object[] parameters)
        {
            // we should make sure we can deserialize the result we get
            if (methodHandler.TryGetSerializer(methodPointer, out IPacketSerializer<object[]> serializer) is false) throw MissingParameterSerializer(methodPointer.ToString());

            var taskSource = new TaskCompletionSource<bool>();

            void InvokeCallback(bool success, IPacket response)
            {
                taskSource.SetResult(success);
            }

            var packet = new PointerOperationPacket(PointerOperations.Invoke,
                wrappedPacket: new CallbackPacket(
                    callback: InvokeCallback,
                    packet: new InvokePointerWithParametersPacket(methodPointer, serializer, parameters),
                    packetCallbackHandler: callbackHandler
                )
            );

            sender.Send(packet);

            return taskSource.Task;
        }

        public Task<T> InvokeStatic<T>(INetPtr<T> methodPointer)
        {
            // we should make sure we can deserialize the result we get
            if (typeHandler.TryGetSerializableType<T>(out ISerializableType<T> deserializer) is false) throw MissingSerializerException(typeof(T).FullName);

            var taskSource = new TaskCompletionSource<T>();

            void InvokeCallback(bool success, IPacket response)
            {
                try
                {
                    if (success)
                    {
                        taskSource.SetResult(deserializer.Deserialize(response));
                    }
                    else
                    {
                        taskSource.SetResult(default);
                    }
                }
                catch (Exception e)
                {
                    taskSource.SetException(e);
                }
            }

            var packet = new PointerOperationPacket(PointerOperations.Invoke,
                wrappedPacket: new CallbackPacket(
                    callback: InvokeCallback,
                    packet: methodPointer,
                    packetCallbackHandler: callbackHandler
                )
            );

            sender.Send(packet);

            return taskSource.Task;
        }

        public Task<T> InvokeStatic<T>(INetPtr<T> methodPointer, params object[] parameters)
        {
            // we should make sure we can deserialize the result we get
            if (methodHandler.TryGetSerializer(methodPointer, out IPacketSerializer<object[]> serializer) is false) throw MissingParameterSerializer(methodPointer.ToString());
            // we should make sure we can deserialize the result we get
            if (typeHandler.TryGetSerializableType<T>(out ISerializableType<T> deserializer) is false) throw MissingSerializerException(typeof(T).FullName);

            var taskSource = new TaskCompletionSource<T>();

            void InvokeCallback(bool success, IPacket response)
            {
                try
                {
                    if (success)
                    {
                        taskSource.SetResult(deserializer.Deserialize(response));
                    }
                    else
                    {
                        taskSource.SetResult(default);
                    }
                }
                catch (Exception e)
                {
                    taskSource.SetException(e);
                }
            }

            var packet = new PointerOperationPacket(PointerOperations.Invoke,
                wrappedPacket: new CallbackPacket(
                    callback: InvokeCallback,
                    packet: new InvokePointerWithParametersPacket(methodPointer, serializer, parameters),
                    packetCallbackHandler: callbackHandler
                )
            );

            sender.Send(packet);

            return taskSource.Task;
        }
        #endregion Invoke

        private Exception MissingTypeException(string typeName)
        {
            return new MissingMemberException($"The type {typeName} was not found within the {nameof(ITypeHandler)} as a registered type. Use {nameof(ITypeHandler.RegisterType)} to register the type before attempting to use it {typeName}.");
        }
        private Exception MissingSerializerException(string typeName)
        {
            return new MissingMemberException($"The type {typeName} was not found within the {nameof(ITypeHandler)} as a serializable and/or deserializable type.");
        }
        private Exception MissingParameterSerializer(string methodPointer)
        {
            return new MissingMemberException($"The method {methodPointer} is missing some or all serializers for some or all of it's parameters. Verify that they are registered with the {nameof(IMethodHandler)}.");
        }
    }
}
