using System;
using System.Collections.Generic;
using System.Text;

namespace NetInterop.Abstractions
{
    public interface IDeactivator
    {
        void DestroyInstance(ref object instance);
    }
    /// <summary>
    /// Responsible for disposing of instances of type <typeparamref name="T"/>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IDeactivator<T> : IDeactivator
    {
        /// <summary>
        /// Destroys, disposes, or frees <paramref name="instance"/>
        /// </summary>
        /// <param name="instance">the instance of <typeparamref name="T"/> that should be disposed, destroyed, freed</param>
        void DestroyInstance(ref T instance);
    }
}
