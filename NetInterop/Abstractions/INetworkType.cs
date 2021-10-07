using NetInterop.Attributes;
namespace NetInterop
{
    public interface INetworkType
    {
        /// <summary>
        /// The unique identifier that was either explicitly provided when registered, or automatically retrieved from an <see cref="InteropIdAttribute"/>
        /// </summary>
        ushort InteropId { get; }

        /// <summary>
        /// Creates a new instance of the type and returns a <see cref="INetPtr"/> that identifies the object within this type
        /// </summary>
        /// <returns></returns>
        INetPtr AllocPtr();

        /// <summary>
        /// Frees the provided ptr for re-use, for <see cref="System.ValueType"/>s only the ptr is freed, the object becomes orphaned, for reference types, if they implement IDisposable
        /// are automatically disposed, and if disposer behaviour was provided to this object then the disposer is ran with the reference of the object
        /// being freed.
        /// </summary>
        /// <param name="ptr"></param>
        void Free(INetPtr ptr);

        /// <summary>
        /// Gets an abiguois object at the provided pointer, if no object is at the provided pointer then null is returned
        /// </summary>
        /// <param name="ptr"></param>
        /// <returns></returns>
        object GetPtr(INetPtr ptr);

        /// <summary>
        /// Sets the value at the provided pointer
        /// </summary>
        /// <param name="ptr"></param>
        /// <param name="value"></param>
        void SetPtr(INetPtr ptr, object value);

        /// <summary>
        /// Frees all managed objects that this controls
        /// </summary>
        void FreeAll();
    }

    public interface INetworkType<T> : INetworkType
    {
        /// <summary>
        /// </summary>
        /// <param name="ptr"></param>
        /// <returns>Returns the value at the network ptr address</returns>
        T GetPtr(INetPtr<T> ptr);

        /// <summary>
        /// Sets the value of the pointer on the remote client
        /// </summary>
        /// <param name="ptr"></param>
        /// <param name="value"></param>
        void SetPtr(INetPtr<T> ptr, T value);
    }
}