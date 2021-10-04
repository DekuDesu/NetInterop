using NetInterop.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetInterop.Runtime.TypeHandling
{
    public class DefaultDeactivator<T> : IDeactivator<T>
    {
        public void DestroyInstance(ref T instance)
        {
            if (instance is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }


        public void DestroyInstance(ref object instance)
        {
            if (instance is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }
    }
}
