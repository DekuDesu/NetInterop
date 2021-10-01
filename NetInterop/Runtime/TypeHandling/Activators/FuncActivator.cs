using NetInterop.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetInterop.Runtime.TypeHandling
{
    /// <summary>
    /// Encapsulates a provided <see cref="Func{TResult}"/> to register it with a <see cref="INetworkTypeHandler"/>
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    public class FuncActivator<TResult> : IActivator<TResult>
    {
        private readonly Func<TResult> expression;

        /// <summary>
        /// Creates an <see cref="IActivator{TResult}"/> decorator over the provided <see cref="Func{TResult}"/>
        /// </summary>
        /// <param name="expression"></param>
        public FuncActivator(Func<TResult> expression)
        {
            this.expression = expression;
        }

        public TResult CreateInstance() => expression();
    }
}
