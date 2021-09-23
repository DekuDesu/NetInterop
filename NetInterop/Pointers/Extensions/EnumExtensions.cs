using System;
using System.Collections.Generic;
using System.Text;

namespace NetInterop.Extensions
{
    public static class EnumExtensions
    {
        public static bool Contains(this PointerResponses container, PointerResponses value)
        {
            return (container & value) != 0;
        }
    }
}
