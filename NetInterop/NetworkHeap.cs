using NetInterop.Transport.Core.Abstractions.Packets;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetInterop
{
    public class NetworkHeap<TPacket> where TPacket : Enum, IConvertible
    {
        private readonly INetworkTypeHandler typeHandler;
        private readonly IPacketSender<TPacket> sender;

        public IPointerResponseHandler PointerResponseHandler { get; set; }

        public NetworkHeap(INetworkTypeHandler typeHandler, IPacketSender<TPacket> sender)
        {
            this.typeHandler = typeHandler;
            this.sender = sender;
        }

        public IPointerProvider PointerProvider { get; set; } = new DefaultPointerProvider();
        public IDelegateHandler<bool, IPacket> CallbackDelegateHandler { get; set; } = new DefaultPacketDelegateHandler();

        public void Create<T>(Action<INetPtr<T>> callback)
        {
            if (typeHandler.TryGetTypePtr<T>(out ushort typePtr))
            {
                INetPtr<T> ptr = PointerProvider.Create<T>(typePtr, 0);

                sender.Send(new PointerOperationPacket<TPacket>(PointerOperations.Alloc, new CallbackPacket<TPacket>(
                    (goodResponse, packet) =>
                    {
                        if (goodResponse)
                        {
                            callback(PointerProvider.Deserialize(packet).As<T>());
                        }
                    },
                    ptr, CallbackDelegateHandler)));
                return;
            }
        }

        public void Destroy<T>(INetPtr<T> ptr)
        {
            sender.Send(new PointerOperationPacket<TPacket>(PointerOperations.Free, new CallbackPacket<TPacket>(null, ptr, CallbackDelegateHandler)));
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
                    CallbackDelegateHandler
                )));
                return;
            }
            throw new InvalidOperationException($"Failed to get a remote instance of type {typeof(T).FullName}, it was not registered in the {nameof(typeHandler)}, use INetworkTypeHandler.Register<T>(id) to register the type as a network type ensure a valid IPacketDeserializer is registered to properly deserialize the type.");
        }

        public void Set<T>(INetPtr<T> ptr, T value)
        {
            if (typeHandler.TryGetSerializableType<T>(ptr, out var serializer))
            {
                sender.Send(new PointerOperationPacket<TPacket>(PointerOperations.Get, new CallbackPacket<TPacket>(
                    null,
                    new SetPointerPacket<T>(ptr, value, serializer),
                    CallbackDelegateHandler
                )));
                return;
            }
            throw new InvalidOperationException($"Failed to get a remote instance of type {typeof(T).FullName}, it was not registered in the {nameof(typeHandler)}, use INetworkTypeHandler.Register<T>(id) to register the type as a network type ensure a valid IPacketDeserializer is registered to properly deserialize the type.");
        }

        public void Invoke<T>(INetPtr<T> ptr)
        {
            throw new NotImplementedException();
        }
    }
}
