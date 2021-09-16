using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetInterop.Transport.Core.Delegates
{
    public delegate Span<T> ToSpanFunc<T>();
    public delegate Span<TResult> ToSpanFunc<T, TResult>(T arg);
    public delegate Span<TResult> ToSpanFunc<T1, T2, TResult>(T1 arg0, T2 arg1);
    public delegate Span<TResult> ToSpanFunc<T1, T2, T3, TResult>(T1 arg0, T2 arg1, T3 arg2);
    public delegate Span<TResult> ToSpanFunc<T1, T2, T3, T4, TResult>(T1 arg0, T2 arg1, T3 arg2, T4 arg3);
    public delegate Span<TResult> ToSpanFunc<T1, T2, T3, T4, T5, TResult>(T1 arg0, T2 arg1, T3 arg2, T4 arg3, T5 arg4);

    public delegate TResult FromSpanFunc<T, TResult>(Span<T> span);
    public delegate TResult FromSpanFunc<T1, T2, TResult>(Span<T1> span, Span<T2> span1);
    public delegate TResult FromSpanFunc<T1, T2, T3, TResult>(Span<T1> span, Span<T2> span1, Span<T3> span2);
}
