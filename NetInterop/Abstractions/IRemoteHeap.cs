using NetInterop.Transport.Core.Abstractions.Packets;
using System;
using System.Threading.Tasks;

namespace NetInterop
{
    /// <summary>
    /// Controls registered interop types on a remote client
    /// </summary>
    [System.Obsolete("Migrate to IRemoteHeap instead")]
    public interface INetworkHeap
    {
        IPointerProvider PointerProvider { get; set; }

        /// <summary>
        /// Instantiates an instance of <see cref="{T}"/> on the remote client. 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="callback"></param>
        void Create<T>(Action<INetPtr<T>> callback);

        /// <summary>
        /// Instantiates an instance of the object who's type is defined within the provided <see cref="INetPtr"/>, on the remote client. 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="callback"></param>
        void Create(INetPtr typePtr, Action<INetPtr> callback);

        /// <summary>
        /// Destroyes and Diposes the object on the remote client, if defined. For value types where no instantiator, or disposer are defined when they are registered, they're <see cref="INetPtr"/> addresses are made available for re-use - but their values remain until written over with new data.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ptr"></param>
        void Destroy<T>(INetPtr<T> ptr);

        /// <summary>
        /// Destroyes and Diposes the object on the remote client, if defined. For value types where no instantiator, or disposer are defined when they are registered, they're <see cref="INetPtr"/> addresses are made available for re-use - but their values remain until written over with new data.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ptr"></param>
        void Destroy(INetPtr ptr);

        /// <summary>
        /// Retrieves a value from the remote client
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ptr"></param>
        /// <param name="callback"></param>
        void Get<T>(INetPtr<T> ptr, Action<T> callback);

        /// <summary>
        /// Retrieves a value from the remote client
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ptr"></param>
        /// <param name="callback"></param>
        void Get(INetPtr ptr, Action<object> callback);

        /// <summary>
        /// Invokes the provided static method pointer on the remote client with no parameters
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ptr"></param>
        void InvokeStatic(INetPtr methodPtr);

        /// <summary>
        /// Invokes the provided static method pointer on the remote client with no parameters and invokes the callback with the result of the method
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ptr"></param>
        void InvokeStatic<T>(INetPtr methodPtr, Action<T> callback);

        /// <summary>
        /// Invokes the provided static method pointer on the remote client with no parameters and invokes the callback with the result of the method
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ptr"></param>
        void InvokeStatic(INetPtr methodPtr, params object[] parameters);

        /// <summary>
        /// Invokes the provided static method pointer on the remote client with no parameters and invokes the callback with the result of the method
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ptr"></param>
        void Invoke<T>(INetPtr methodPtr, Action<T> callback, params object[] parameters);

        /// <summary>
        /// Invokes the provided method on the provided instance with no parameters
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ptr"></param>
        void Invoke<T>(INetPtr methodPtr, INetPtr<T> instancePtr);

        /// <summary>
        /// Invokes the provided method on the provided instance with no parameters
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ptr"></param>
        void Invoke(INetPtr methodPtr, INetPtr instancePtr);

        /// <summary>
        /// Invokes the provided method on the provided instance with no paramaters, when the method is invoked the callback is invoked with the return value of the method
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ptr"></param>
        void Invoke<T>(INetPtr methodPtr, INetPtr<T> instancePtr, Action<T> callback);

        /// <summary>
        /// Invokes the provided method on the provided instance with parameters
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ptr"></param>
        void Invoke<T>(INetPtr methodPtr, INetPtr<T> instancePtr, params object[] parameters);

        /// <summary>
        /// Invokes the provided method on the provided instance with parameters, when the method is invoked the callback is invoked with the return value of the method
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ptr"></param>
        void Invoke<T>(INetPtr methodPtr, INetPtr<T> instancePtr, Action<T> callback, params object[] parameters);

        /// <summary>
        /// Sets the value at the provided <see cref="INetPtr"/> on the remote client
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ptr"></param>
        /// <param name="value"></param>
        void Set<T>(INetPtr<T> ptr, T value);

        /// <summary>
        /// Sets the value at the provided <see cref="INetPtr"/> on the remote client
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ptr"></param>
        /// <param name="value"></param>
        void Set(INetPtr ptr, object value);
    }
    public interface IRemoteHeap
    {
        Task<INetPtr<T>> Create<T>();
        Task<INetPtr> Create(INetPtr typePointer);
        Task<INetPtr<T>> Create<T>(INetPtr<T> typePointer);

        Task<T> Get<T>(INetPtr<T> instancePointer);
        Task<object> Get(INetPtr instancePointer);

        Task<bool> Set<T>(INetPtr<T> instancePointer, T value);
        Task<bool> Set(INetPtr instancePointer, object value);

        Task<bool> Destroy<T>(INetPtr<T> instancePointer);
        Task<bool> Destroy(INetPtr instancePointer);

        Task<bool> InvokeStatic(INetPtr methodPointer);
        Task<bool> InvokeStatic(INetPtr methodPointer, params object[] parameters);

        Task<T> InvokeStatic<T>(INetPtr<T> methodPointer);
        Task<T> InvokeStatic<T>(INetPtr<T> methodPointer, params object[] parameters);

        Task<bool> Invoke(INetPtr methodPointer, INetPtr instancePointer);
        Task<T> Invoke<T>(INetPtr<T> methodPointer, INetPtr instancePointer);

        Task<bool> Invoke(INetPtr methodPointer, INetPtr instancePointer, params object[] parameters);
        Task<T> Invoke<T>(INetPtr<T> methodPointer, INetPtr instancePointer, params object[] parameters);
    }
}