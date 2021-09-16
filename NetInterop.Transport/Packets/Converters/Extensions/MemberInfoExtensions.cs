using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace NetInterop.Transport.Core.Packets
{
    public static class MemberInfoExtensions
    {
        public static bool Contains(this MemberTypes container, MemberTypes value)
        {
            return (container & value) != 0;
        }
    }
}
