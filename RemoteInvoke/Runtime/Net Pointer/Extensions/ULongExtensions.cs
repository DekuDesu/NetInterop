using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemoteInvoke.Runtime.NetPtr.Extensions
{
    public static class ULongExtensions
    {
        public static bool Contains(this ulong ptr, ulong mask)
        {
            return (ptr | mask) != mask;
        }
    }
}
