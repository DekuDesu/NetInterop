using System;
using System.Collections.Generic;
using System.Text;

namespace NetInterop.Abstractions
{
    public interface IObjectHeap
    {
        INetPtr Alloc(INetPtr ptr);
        INetPtr<T> Alloc<T>(INetPtr<T> ptr);

        void Set(INetPtr instancePtr, object value);
        void Set<T>(INetPtr<T> instancePtr, T value);

        object Get(INetPtr instancePtr);
        T Get<T>(INetPtr<T> instancePtr);

        void Free(INetPtr instancePtr);

        void Clear();
    }
    public interface IObjectHeap<T> : IObjectHeap
    {
        INetPtr<T> Alloc();
        void Set(INetPtr<T> instancePtr, T value);
        T Get(INetPtr<T> instancePtr);
    }
}
