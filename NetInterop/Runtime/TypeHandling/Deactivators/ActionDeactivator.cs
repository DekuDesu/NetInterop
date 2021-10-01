using NetInterop.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetInterop.Runtime.TypeHandling
{
    public class ActionDeactivator<T> : IDeactivator<T>
    {
        private readonly Action<T> expression;

        /// <summary>
        /// Creates an <see cref="IDeactivator{T}"/> decorator around the provided <see cref="Action{T}"/>
        /// </summary>
        /// <param name="expression"></param>
        public ActionDeactivator(Action<T> expression)
        {
            this.expression = expression ?? throw new ArgumentNullException(nameof(expression));
        }

        public void DestroyInstance(T instance) => expression(instance);
    }
}
