using System;
using System.Collections.Generic;
using System.Text;

namespace NetInterop.Transport.Core.Abstractions
{
    /// <summary>
    /// Represents some object that takes in some dependency <see cref="{T}"/>, may or may not modify or replace it, and returns a dependency of type <see cref="{T}"/>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IDependencyModifier<T>
    {
        /// <summary>
        /// Modifies or replaces <paramref name="value"/> and returns it
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        T Inject(T value);
    }
}
