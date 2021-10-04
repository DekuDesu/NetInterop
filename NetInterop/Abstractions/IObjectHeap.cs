using System;
using System.Collections.Generic;
using System.Text;

namespace NetInterop.Abstractions
{
    public interface IObjectHeap
    {
        INetPtr Alloc(INetPtr ptr);
        void Set(INetPtr instancePtr, object value);
        object Get(INetPtr instancePtr);
        void Free(INetPtr instancePtr);
        void Clear();
    }
    public interface IObjectHeap<T> : IObjectHeap
    {
        void Set(INetPtr instancePtr, T value);
        new T Get(INetPtr instancePtr);
    }
}
