using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace NetInterop.Runtime.Extensions
{
    public static class MethodHandlingExtensions
    {
        public static INetPtr Register<T>(this INetworkMethodHandler handler, Func<T, string> nameExpression)
        {
            return handler.Register(typeof(T).GetMethod(nameExpression(default), System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static));
        }
    }
}
