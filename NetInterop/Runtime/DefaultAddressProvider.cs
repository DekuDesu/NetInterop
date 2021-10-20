using NetInterop.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetInterop
{
    public class DefaultAddressProvider : IAddressProvider
    {
        private ushort currentAddress;

        public ushort GetNewAddress()
        {
            return currentAddress++;
        }
    }
}
