using NetInterop.Transport.Core.Abstractions.Packets;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetInterop.Abstractions
{
    /// <summary>
    /// Represents a registered <see cref="NetInterop"/> runtime type
    /// </summary>
    public interface IType
    {
        Type BackingType { get; }
        /// <summary>
        /// The unique identifier that was either explicitly provided when registered, or automatically retrieved from an <see cref="InteropIdAttribute"/>
        /// </summary>
        INetPtr TypePointer { get; }

        /// <summary>
        /// Attempts to dispose and deacvtivate the object if applicable
        /// </summary>
        /// <param name="ptr"></param>
        void Deactivate(ref object instance);

        /// <summary>
        /// Creates a new instance of the type and returns a <see cref="INetPtr"/> that identifies the object within this type
        /// </summary>
        /// <returns></returns>
        object Activate();
    }

    /// <summary>
    /// Represents a strongly type <see cref="IType"/>, which represents a registered <see cref="NetInterop"/> runtime type
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IType<T> : IType
    {
        /// <summary>
        /// Attempts to dispose and deacvtivate the object if applicable
        /// </summary>
        /// <param name="ptr"></param>
        void Deactivate(ref T instance);
        /// <summary>
        /// Creates a new instance of the type and returns a <see cref="INetPtr"/> that identifies the object within this type
        /// </summary>
        /// <returns></returns>
        new T Activate();
    }

    /// <summary>
    /// Represents a ambigiously typed and fully network serializable <see cref="ISerializableType{T}"/>, which represents a registered <see cref="NetInterop"/> runtime type
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ISerializableType : IType, IPacketSerializer, IPacketDeserializer { }

    /// <summary>
    /// Represents a strongly typed and fully network serializable <see cref="ISerializableType{T}"/>, which represents a registered <see cref="NetInterop"/> runtime type
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ISerializableType<T> : ISerializableType, IType<T>, IPacketSerializer<T>, IPacketDeserializer<T> { }
}
