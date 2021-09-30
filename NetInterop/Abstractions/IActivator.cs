using System;
using System.Collections.Generic;
using System.Text;

namespace NetInterop.Abstractions
{
    /// <summary>
    /// Defines an object that is responsible for creating new instances of <typeparamref name="TResult"/>
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    public interface IActivator<TResult>
    {
        /// <returns>an instance of type <typeparamref name="TResult"/></returns>
        TResult CreateInstance();
    }
}
