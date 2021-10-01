using NetInterop.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetInterop.Runtime.TypeHandling
{
    public class DefaultActivator<T> : IActivator<T>
    {
        public T CreateInstance() => Activator.CreateInstance<T>();
    }
}
