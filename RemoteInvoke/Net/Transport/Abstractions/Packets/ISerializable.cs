using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemoteInvoke.Net.Transport.Abstractions
{
    /// <summary>
    /// Defines an object implements functionality to serialize itself into <typeparamref name="TResult"/>
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    public interface ISerializable<TResult>
    {
        /// <summary>
        /// Serializes this object to <typeparamref name="TResult"/>
        /// </summary>
        /// <returns></returns>
        TResult Serialize();
    }
}
