using NetInterop.Transport.Core.Abstractions.Packets;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetInterop
{
    public class NetworkHeap<TPacket> : INetworkHeap where TPacket : Enum, IConvertible
    {
        private readonly INetworkMethodHandler methodHandler;
        private readonly INetworkTypeHandler typeHandler;
        private readonly IPacketSender<TPacket> sender;
        private readonly IDelegateHandler<bool, IPacket> callbackDelegateHandler;
        public NetworkHeap(INetworkTypeHandler typeHandler, IPacketSender<TPacket> sender, IDelegateHandler<bool, IPacket> callbackDelegateHandler, INetworkMethodHandler methodHandler)
        {
            this.typeHandler = typeHandler ?? throw new ArgumentNullException(nameof(typeHandler));
            this.sender = sender ?? throw new ArgumentNullException(nameof(sender));
            this.callbackDelegateHandler = callbackDelegateHandler ?? throw new ArgumentNullException(nameof(callbackDelegateHandler));
            this.methodHandler = methodHandler ?? throw new ArgumentNullException(nameof(methodHandler));
        }

        public IPointerProvider PointerProvider { get; set; } = new DefaultPointerProvider();

        public void Create<T>(Action<INetPtr<T>> callback)
        {
            if (typeHandler.TryGetTypePtr<T>(out INetPtr<T> ptr))
            {
                sender.Send(new PointerOperationPacket<TPacket>(PointerOperations.Alloc, new CallbackPacket<TPacket>(
                    (goodResponse, packet) =>
                    {
                        if (goodResponse)
                        {
                            callback(PointerProvider.Deserialize(packet).As<T>());
                        }
                    },
                    ptr, callbackDelegateHandler)));
                return;
            }
        }

        public void Create(INetPtr typePtr, Action<INetPtr> callback)
        {
            sender.Send(new PointerOperationPacket<TPacket>(PointerOperations.Alloc, new CallbackPacket<TPacket>(
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
            sender.Send(new PointerOperationPacket<TPacket>(PointerOperations.Free, new CallbackPacket<TPacket>(null, ptr, callbackDelegateHandler)));
        }

        public void Destroy(INetPtr ptr)
        {
            sender.Send(new PointerOperationPacket<TPacket>(PointerOperations.Free, new CallbackPacket<TPacket>(null, ptr, callbackDelegateHandler)));
        }

        public void Get<T>(INetPtr<T> ptr, Action<T> callback)
        {
            if (typeHandler.TryGetSerializableType<T>(ptr, out var serializer))
            {
                sender.Send(new PointerOperationPacket<TPacket>(PointerOperations.Get, new CallbackPacket<TPacket>(
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
            if (typeHandler.TryGetAmbiguousSerializableType(ptr, out var serializer))
            {
                sender.Send(new PointerOperationPacket<TPacket>(PointerOperations.Get, new CallbackPacket<TPacket>(
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
            if (typeHandler.TryGetSerializableType<T>(ptr, out var serializer))
            {
                sender.Send(new PointerOperationPacket<TPacket>(PointerOperations.Set, new CallbackPacket<TPacket>(
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
            if (typeHandler.TryGetAmbiguousSerializableType(ptr, out var serializer))
            {
                sender.Send(new PointerOperationPacket<TPacket>(PointerOperations.Set, new CallbackPacket<TPacket>(
                    null,
                    new SetAmbiguousPointerPacket(ptr, value, serializer),
                    callbackDelegateHandler
                )));
                return;
            }
            throw new InvalidOperationException($"Failed to get a remote instance of net interop pointer {ptr}, it was not registered in the {nameof(typeHandler)}, use INetworkTypeHandler.Register<T>(id) to register the type as a network type ensure a valid IPacketDeserializer is registered to properly deserialize the type.");
        }

        public void Invoke(INetPtr methodPtr)
        {
            sender.Send(new PointerOperationPacket<TPacket>(PointerOperations.Invoke, new CallbackPacket<TPacket>(null, methodPtr, callbackDelegateHandler)));
        }

        public void Invoke<T>(INetPtr methodPtr, Action<T> callback)
        {
            if (typeHandler.TryGetSerializableType<T>(out var serializer))
            {
                sender.Send(new PointerOperationPacket<TPacket>(PointerOperations.Invoke, new CallbackPacket<TPacket>(
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
                return;
            }
            throw new InvalidOperationException($"Failed to invoke and retrieve a value for a remote instance of type {typeof(T).FullName}, it was not registered in the {nameof(typeHandler)}, use INetworkTypeHandler.Register<T>(id) to register the type as a network type ensure a valid IPacketDeserializer is registered to properly deserialize the type.");
        }

        public void Invoke<T>(INetPtr methodPtr, params object[] parameters)
        {

        }

        public void Invoke<T>(INetPtr methodPtr, Action<T> callback, params object[] parameters)
        {
            if (typeHandler.TryGetSerializableType<T>(out var serializer))
            {
                sender.Send(new PointerOperationPacket<TPacket>(PointerOperations.Invoke, new CallbackPacket<TPacket>(
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
                return;
            }
            throw new InvalidOperationException($"Failed to invoke and retrieve a value for a remote instance of type {typeof(T).FullName}, it was not registered in the {nameof(typeHandler)}, use INetworkTypeHandler.Register<T>(id) to register the type as a network type ensure a valid IPacketDeserializer is registered to properly deserialize the type.");
        }

        public void Invoke<T>(INetPtr methodPtr, INetPtr<T> instancePtr)
        {
            throw new NotImplementedException();
        }

        public void Invoke(INetPtr methodPtr, INetPtr instancePtr)
        {
            throw new NotImplementedException();
        }

        public void Invoke<T>(INetPtr methodPtr, INetPtr<T> instancePtr, Action<T> callback)
        {
            throw new NotImplementedException();
        }

        public void Invoke<T>(INetPtr methodPtr, INetPtr<T> instancePtr, params object[] parameters)
        {
            throw new NotImplementedException();
        }

        public void Invoke<T>(INetPtr methodPtr, INetPtr<T> instancePtr, Action<T> callback, params object[] parameters)
        {
            throw new NotImplementedException();
        }
    }
}
