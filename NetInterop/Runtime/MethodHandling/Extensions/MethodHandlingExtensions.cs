using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace NetInterop.Runtime.Extensions
{
    public static class MethodHandlingExtensions
    {
        private const string FailedToConvertExpressionMessage = "Failed to register method or delegate becuase the expression '{0}' was not supported or understood.";
        private static MethodInfo GetMethodFromExpression(Expression expression)
        {
            if (expression is UnaryExpression unaryExpression)
            {
                if (unaryExpression.Operand is MethodCallExpression methodCall)
                {
                    if (methodCall.Object is ConstantExpression constantExpression)
                    {
                        if (constantExpression.Value is MethodInfo info)
                        {
                            return info;
                        }
                    }
                }
            }

            string message = string.Format(FailedToConvertExpressionMessage, expression);

            throw new ArgumentException(message, nameof(expression));
        }

        public static INetPtr<TResult> Register<TResult>(this IMethodHandler handler, MethodInfo method) => handler.Register<TResult>(method);
        public static INetPtr<TResult> Register<TResult>(this IMethodHandler handler, Func<TResult> staticMethod) => handler.Register<TResult>(staticMethod.Method);
        public static INetPtr<TResult> Register<T, TResult>(this IMethodHandler handler, Func<T, TResult> staticMethod) => handler.Register<TResult>(staticMethod.Method);
        public static INetPtr<TResult> Register<T1, T2, TResult>(this IMethodHandler handler, Func<T1, T2, TResult> staticMethod) => handler.Register<TResult>(staticMethod.Method);
        public static INetPtr<TResult> Register<T1, T2, T3, TResult>(this IMethodHandler handler, Func<T1, T2, T3, TResult> staticMethod) => handler.Register<TResult>(staticMethod.Method);
        public static INetPtr<TResult> Register<T1, T2, T3, T4, TResult>(this IMethodHandler handler, Func<T1, T2, T3, T4, TResult> staticMethod) => handler.Register<TResult>(staticMethod.Method);
        public static INetPtr<TResult> Register<T1, T2, T3, T4, T5, TResult>(this IMethodHandler handler, Func<T1, T2, T3, T4, T5, TResult> staticMethod) => handler.Register<TResult>(staticMethod.Method);

        public static INetPtr Register(this IMethodHandler handler, Action staticMethod) => handler.Register(staticMethod.Method);
        public static INetPtr Register<T>(this IMethodHandler handler, Action<T> staticMethod) => handler.Register(staticMethod.Method);
        public static INetPtr Register<T1, T2>(this IMethodHandler handler, Action<T1, T2> staticMethod) => handler.Register(staticMethod.Method);
        public static INetPtr Register<T1, T2, T3>(this IMethodHandler handler, Action<T1, T2, T3> staticMethod) => handler.Register(staticMethod.Method);
        public static INetPtr Register<T1, T2, T3, T4>(this IMethodHandler handler, Action<T1, T2, T3, T4> staticMethod) => handler.Register(staticMethod.Method);
        public static INetPtr Register<T1, T2, T3, T4, T5>(this IMethodHandler handler, Action<T1, T2, T3, T4, T5> staticMethod) => handler.Register(staticMethod.Method);


        public static INetPtr<TResult> Register<TDeclaring, TResult>(this IMethodHandler handler, Expression<Func<TDeclaring, Func<TResult>>> expression) => handler.Register<TResult>(GetMethodFromExpression(expression.Body));
        public static INetPtr<TResult> Register<TDeclaring, T, TResult>(this IMethodHandler handler, Expression<Func<TDeclaring, Func<T, TResult>>> expression) => handler.Register<TResult>(GetMethodFromExpression(expression.Body));
        public static INetPtr<TResult> Register<TDeclaring, T1, T2, TResult>(this IMethodHandler handler, Expression<Func<TDeclaring, Func<T1, T2, TResult>>> expression) => handler.Register<TResult>(GetMethodFromExpression(expression.Body));
        public static INetPtr<TResult> Register<TDeclaring, T1, T2, T3, TResult>(this IMethodHandler handler, Expression<Func<TDeclaring, Func<T1, T2, T3, TResult>>> expression) => handler.Register<TResult>(GetMethodFromExpression(expression.Body));
        public static INetPtr<TResult> Register<TDeclaring, T1, T2, T3, T4, TResult>(this IMethodHandler handler, Expression<Func<TDeclaring, Func<T1, T2, T3, T4, TResult>>> expression) => handler.Register<TResult>(GetMethodFromExpression(expression.Body));
        public static INetPtr<TResult> Register<TDeclaring, T1, T2, T3, T4, T5, TResult>(this IMethodHandler handler, Expression<Func<TDeclaring, Func<T1, T2, T3, T4, T5, TResult>>> expression) => handler.Register<TResult>(GetMethodFromExpression(expression.Body));

        public static INetPtr Register<TDeclaring>(this IMethodHandler handler, Expression<Func<TDeclaring, Action>> expression) => handler.Register(GetMethodFromExpression(expression.Body));
        public static INetPtr Register<TDeclaring, T>(this IMethodHandler handler, Expression<Func<TDeclaring, Action<T>>> expression) => handler.Register(GetMethodFromExpression(expression.Body));
        public static INetPtr Register<TDeclaring, T1, T2>(this IMethodHandler handler, Expression<Func<TDeclaring, Action<T1, T2>>> expression) => handler.Register(GetMethodFromExpression(expression.Body));
        public static INetPtr Register<TDeclaring, T1, T2, T3>(this IMethodHandler handler, Expression<Func<TDeclaring, Action<T1, T2, T3>>> expression) => handler.Register(GetMethodFromExpression(expression.Body));
        public static INetPtr Register<TDeclaring, T1, T2, T3, T4>(this IMethodHandler handler, Expression<Func<TDeclaring, Action<T1, T2, T3, T4>>> expression) => handler.Register(GetMethodFromExpression(expression.Body));
        public static INetPtr Register<TDeclaring, T1, T2, T3, T4, T5>(this IMethodHandler handler, Expression<Func<TDeclaring, Action<T1, T2, T3, T4, T5>>> expression) => handler.Register(GetMethodFromExpression(expression.Body));
    }
}
