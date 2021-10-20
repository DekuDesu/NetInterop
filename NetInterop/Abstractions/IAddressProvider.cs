using System;
using System.Collections.Generic;
using System.Text;

namespace NetInterop.Abstractions
{
    /// <summary>
    /// Responsible for deterministically generating/providing addresses for type registration during runtime
    /// </summary>
    public interface IAddressProvider
    {
        /// <summary>
        /// Generates/provides a new unique address
        /// </summary>
        /// <returns></returns>
        ushort GetNewAddress();
    }
}
