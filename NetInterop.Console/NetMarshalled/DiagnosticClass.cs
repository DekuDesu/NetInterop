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
    public class DiagnosticClass : IDisposable
    {
        public int Value { get; set; }

        public static INetPtr SetValuePointer { get; private set; }
        public void SetValue(int value)
        {
            Value = value;
        }

        public static INetPtr<int> GetValuePointer { get; private set; }
        public int GetValue()
        {
            return Value;
        }

        public static INetPtr WritePointer { get; private set; }
        public void Write(string message)
        {
            Console.WriteLine($"TestClass: {message}");
        }

        public void Dispose()
        {
            Console.WriteLine("Disposed TestClass Instance");
        }

        public static INetPtr<DiagnosticClass> TypePointer { get; private set; }

        public static void RegisterType(ITypeHandler handler, ref ushort address)
        {
            TypePointer = handler.RegisterType<DiagnosticClass>(address++, DiagnosticClassSerializer.Instance, DiagnosticClassSerializer.Instance, DiagnosticClassSerializer.Instance, DiagnosticClassSerializer.Instance);
        }

        public static void RegisterMethods(IMethodHandler handler)
        {
            WritePointer = handler.Register<DiagnosticClass, string>(x => x.Write);
            GetValuePointer = handler.Register<DiagnosticClass, int>(x => x.GetValue);
            SetValuePointer = handler.Register<DiagnosticClass, int>(x => x.SetValue);
        }
    }
}
