using NetInterop.Abstractions;
using NetInterop.Example.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetInterop.Runtime.Extensions;

namespace NetInterop.Example
{
    public class HelloWorld
    {
        public string Message { get; init; } = "Hello";

        public static INetPtr m_LogMessage { get; private set; }
        public void LogMessage(string message)
        {
            Console.Write($"{message}\n\r");
        }

        public static INetPtr m_Log { get; private set; }
        public void Log()
        {
            Console.Write(Message);
        }

        public static void RegisterType(ITypeHandler handler, IAddressProvider provider)
        {
            handler.RegisterType<HelloWorld, HelloWorldSerializer>(provider.GetNewAddress());
        }

        public static void RegisterMethods(IMethodHandler handler)
        {
            m_LogMessage = handler.Register<HelloWorld, string>(x => x.LogMessage);
            m_Log = handler.Register<HelloWorld>(x => x.Log);
        }
    }
}
