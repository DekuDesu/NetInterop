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
        private readonly IProducerConsumerCollection<Barrier> barriers = new ConcurrentBag<Barrier>();


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
        public Task Destroy<T>(INetPtr<T> instancePointer)
        {
            throw new NotImplementedException();
        }

        public Task Destroy(INetPtr instancePointer)
        {
            throw new NotImplementedException();
        }
        #endregion Destroy

        #region Get
        public Task<T> Get<T>(INetPtr<T> instancePointer)
        {
            throw new NotImplementedException();
        }

        public Task<object> Get(INetPtr instancePointer)
        {
            throw new NotImplementedException();
        }
        #endregion Get

        #region Set
        public Task Set<T>(INetPtr<T> instancePointer, T value)
        {
            throw new NotImplementedException();
        }

        public Task Set(INetPtr instancePointer, object value)
        {
            throw new NotImplementedException();
        }
        #endregion Set

        #region Invoke
        public Task Invoke(INetPtr methodPointer, INetPtr instancePointer)
        {
            throw new NotImplementedException();
        }

        public Task<T> Invoke<T>(INetPtr<T> methodPointer, INetPtr instancePointer)
        {
            throw new NotImplementedException();
        }

        public Task Invoke(INetPtr methodPointer, INetPtr instancePointer, params object[] parameters)
        {
            throw new NotImplementedException();
        }

        public Task<T> Invoke<T>(INetPtr<T> methodPointer, INetPtr instancePointer, params object[] parameters)
        {
            throw new NotImplementedException();
        }

        public Task InvokeStatic(INetPtr methodPointer)
        {
            throw new NotImplementedException();
        }

        public Task InvokeStatic(INetPtr methodPointer, params object[] parameters)
        {
            throw new NotImplementedException();
        }

        public Task<T> InvokeStatic<T>(INetPtr<T> methodPointer)
        {
            throw new NotImplementedException();
        }

        public Task<T> InvokeStatic<T>(INetPtr<T> methodPointer, params object[] parameters)
        {
            throw new NotImplementedException();
        }
        #endregion Invoke

        private Exception MissingTypeException(string typeName)
        {
            return new TypeInitializationException($"The type {typeName} was not found within the {nameof(ITypeHandler)} as a registed type. Use {nameof(ITypeHandler.RegisterType)} to register the type before attempting to use it {typeName}.", null);
        }
    }
}
