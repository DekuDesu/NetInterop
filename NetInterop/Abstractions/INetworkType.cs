namespace NetInterop
{
    public interface INetworkType
    {
        ushort Id { get; }

        INetPtr AllocPtr();

        void Free(INetPtr ptr);

        object GetPtr(INetPtr ptr);

        void SetPtr(INetPtr ptr, object value);

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