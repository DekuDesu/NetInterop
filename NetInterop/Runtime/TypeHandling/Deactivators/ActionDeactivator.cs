using NetInterop.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetInterop.Runtime.TypeHandling
{
    public class ActionDeactivator<T> : IDeactivator<T>
    {
        private readonly RefAction<T> expression;

        /// <summary>
        /// Creates an <see cref="IDeactivator{T}"/> decorator around the provided <see cref="Action{T}"/>
        /// </summary>
        /// <param name="expression"></param>
        public ActionDeactivator(RefAction<T> expression)
        {
            this.expression = expression ?? throw new ArgumentNullException(nameof(expression));
        }

        public void DestroyInstance(ref T instance) => expression(ref instance);

        public void DestroyInstance(ref object instance)
        {
            if (instance is T isT)
            {
                expression(ref isT);
            }
        }
    }
}
