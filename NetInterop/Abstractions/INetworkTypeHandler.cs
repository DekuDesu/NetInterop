using NetInterop.Abstractions;
using NetInterop.Transport.Core.Abstractions.Packets;
using System;

namespace NetInterop
{
    /// <summary>
    /// Handles runtime instantiation, destruction and management of network types during runtime
    /// </summary>
    public interface INetworkTypeHandler
    {
        /// <summary>
        /// Attempts to get find a registerded <see cref="INetworkType"/> using the type or instance pointer provided.
        /// </summary>
        /// <param name="typePtr">Any <see cref="INetPtr"/> with a <see cref="INetPtr.PtrType"/> field populated</param>
        /// <param name="type"></param>
        /// <returns></returns>
        bool TryGetAmbiguousType(INetPtr typePtr, out INetworkType type);
        /// <summary>
        /// Attempts to get find a registerded <see cref="INetworkType"/> using the type or instance pointer provided.
        /// </summary>
        /// <param name="typePtr">Any <see cref="INetPtr"/> with a <see cref="INetPtr.PtrType"/> field populated</param>
        /// <param name="type"></param>
        /// <returns></returns>
        bool TryGetType<T>(INetPtr<T> typePtr, out INetworkType<T> type);
        /// <summary>
        /// Attempts to get find a registerded <see cref="INetworkType"/> using the generic type provided
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        bool TryGetType<T>(out INetworkType<T> type);

        /// <summary>
        /// Attempts to get a <see cref="ISerializableNetworkType"/> using the generic argument
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="serializableNetworkType"></param>
        /// <returns></returns>
        bool TryGetSerializableType<T>(out ISerializableNetworkType<T> serializableNetworkType);

        /// <summary>
        /// Attempts to get an <see cref="ISerializableNetworkType"/> using the <see cref="INetPtr"/> provided
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="typePtr"></param>
        /// <param name="serializableNetworkType"></param>
        /// <returns></returns>
        bool TryGetSerializableType<T>(INetPtr<T> typePtr, out ISerializableNetworkType<T> serializableNetworkType);

        /// <summary>
        /// Attempts to get an <see cref="ISerializableNetworkType"/> using the <see cref="INetPtr"/> provided
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="typePtr"></param>
        /// <param name="serializableNetworkType"></param>
        /// <returns></returns>
        bool TryGetAmbiguousSerializableType(INetPtr typePtr, out ISerializableNetworkType serializableNetworkType);

        /// <summary>
        /// Attempts to find the <see cref="INetPtr"/> that can be used to reference the provided generic argument
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="typePtr"></param>
        /// <returns></returns>
        bool TryGetTypePtr<T>(out INetPtr<T> typePtr);

        /// <summary>
        /// Attempts to find the <see cref="INetPtr"/> that can be used to reference the provided type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="typePtr"></param>
        /// <returns></returns>
        bool TryGetTypePtr(Type type, out INetPtr typePtr);

        /// <summary>
        /// Registers the <see cref="Type"/> with this object allowing it to create, destroy, and manage instaces of that type during runtime between clients over a network
        /// </summary>
        /// <typeparam name="T">The <see cref="Type"/> that should be registered</typeparam>
        /// <param name="interopId">The unique idenfier for this type, Can be between 0x0001 and 0xFFFE</param>
        /// <param name="activator">The object that should create new instances of this type</param>
        /// <param name="disposer">The object that should handle destruction of instances of this type</param>
        /// <param name="serializer">The object that should provide serialization for instances of this type</param>
        /// <param name="deserializer">The object that should provide deserialization for instances of this type</param>
        /// <returns></returns>
        INetPtr<T> RegisterType<T>(ushort interopId, IActivator<T> activator, IDeactivator<T> disposer, IPacketSerializer<T> serializer, IPacketDeserializer<T> deserializer);

        /// <summary>
        /// Clears all registered types from this object
        /// </summary>
        void Clear();
    }
}