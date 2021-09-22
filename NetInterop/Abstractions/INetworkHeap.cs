using System;
using System.Collections.Generic;
using System.Text;

namespace NetInterop
{
    public interface INetworkHeap
    {
        /// <summary>
        /// Creates the provided type on the remote client
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns><see cref="INetPtr"/> network pointer to the object on the remote server</returns>
        INetPtr CreateType<T>();

        /// <summary>
        /// Destroys the target object on the remote client
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ptr"></param>
        void Destroy(INetPtr ptr);

        /// <summary>
        /// Sets the target ptr to the provided value on the remote client
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ptr"></param>
        /// <param name="value"></param>
        void Set<T>(INetPtr ptr, T value);

        /// <summary>
        /// Gets the value of the object on the remote client
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ptr"></param>
        /// <returns></returns>
        T Get<T>(INetPtr ptr);
    }
}
