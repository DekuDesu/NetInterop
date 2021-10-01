using System;
using System.Collections.Generic;
using System.Text;

namespace NetInterop
{
    public enum PointerOperations : byte
    {
        none,
        Callback,
        OperationResult,
        Alloc,
        Free,
        Set,
        Get,
        Invoke
    }
}
