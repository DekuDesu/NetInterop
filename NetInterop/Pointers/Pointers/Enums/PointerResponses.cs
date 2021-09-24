using System;
using System.Collections.Generic;
using System.Text;

namespace NetInterop
{
    public enum PointerResponses : byte
    {
        none,
        GoodResponse = 1,
        BadResponse = 2,
        HasData = 4
    }
}
