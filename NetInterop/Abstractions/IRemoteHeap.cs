using NetInterop.Transport.Core.Abstractions.Packets;
using System;
using System.Threading.Tasks;

namespace NetInterop
{
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