using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetInterop.Transport.Core.Delegates
{
    public delegate void PointerAction(ref byte ptr);
    public delegate void PointerAction<T>(ref byte ptr, T arg);
    public delegate void PointerAction<T1, T2>(ref byte ptr, T1 arg1, T2 arg2);
    public delegate void PointerAction<T1, T2, T3>(ref byte ptr, T1 arg1, T2 arg2, T3 arg3);

    public delegate TResult PointerFunc<TResult>(ref byte ptr);
    public delegate TResult PointerFunc<TResult, T1>(ref byte ptr, T1 arg);
}
