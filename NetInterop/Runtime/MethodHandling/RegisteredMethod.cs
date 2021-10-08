using NetInterop.Abstractions;
using NetInterop.Transport.Core.Abstractions.Packets;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NetInterop.Runtime.MethodHandling
{
    public class RegisteredMethod : IPacketSerializer<object[]>, IPacketDeserializer<object[]>
    {
        private readonly IPointerProvider pointerProvider;
        private readonly IObjectHeap heap;
        private readonly MethodInfo method;
        private readonly MethodParameter[] parameters;
        private readonly MethodParameter returnType;

        public IType DeclaringType { get; private set; }

        public RegisteredMethod(MethodInfo method, MethodParameter returnType, MethodParameter[] parameters, IPointerProvider pointerProvider, IObjectHeap runtimeHeap = null, IType declaringType = null)
        {
            this.method = method ?? throw new ArgumentNullException(nameof(method));
            this.returnType = returnType ?? throw new ArgumentNullException(nameof(returnType));
            this.parameters = parameters ?? throw new ArgumentNullException(nameof(parameters));
            this.pointerProvider = pointerProvider ?? throw new ArgumentNullException(nameof(pointerProvider));
            this.heap = runtimeHeap;
            DeclaringType = declaringType;
        }

        /// <summary>
        /// Deserializes the packet into the <c>this</c> pointer and the various parameters, then invokes the method.
        /// </summary>
        /// <param name="packet"></param>
        /// <param name="packetBuilder"></param>
        public void InvokeVoid(IPacket packet) => Invoke(packet, null);

        /// <summary>
        /// Deserializes the packet into the <c>this</c> pointer and the various parameters, then invokes the method. The result is then serialized and appended to the provided packet
        /// </summary>
        /// <param name="packet"></param>
        /// <param name="packetBuilder"></param>
        public void Invoke(IPacket packet, IPacket packetBuilder)
        {
            object instance;

            try
            {
                instance = GetDeclaringInstance(packet);
            }
            catch (IndexOutOfRangeException)
            {
                throw new IndexOutOfRangeException($"Failed to get an instance of an object to invoke {method.Name} on the remote client. Missing or malformed Net Interop Pointer at the start of the packet.");
            }

            object[] methodParams = Deserialize(packet);

            object result = method.Invoke(instance, methodParams);

            if (result != null && packetBuilder != null)
            {
                returnType.AmbiguousSerialize(result, packetBuilder);
            }
        }

        private object GetDeclaringInstance(IPacket packet)
        {
            if (method.IsStatic)
            {
                return null;
            }

            INetPtr instancePtr = pointerProvider.Deserialize(packet);

            object instance = heap.Get(instancePtr);

            if (instance is null)
            {
                throw new NullReferenceException($"Failed to get an instance of an object for pointer {instancePtr}, a reference to instance of {method.DeclaringType.FullName} is required to invoke {method.Name}");
            }

            return instance;
        }

        private Exception GenerateDetailedParameterException(int parameterIndex)
        {
            StringBuilder builder = new StringBuilder();

            builder.Append($"Failed to deserialize some or all parameters for {method.DeclaringType.FullName}.{method.Name}. Ensure that the parameters are appended in the correct order and not malformed.\n\r\t");

            for (int i = 0; i < parameters.Length; i++)
            {
                if (parameterIndex >= i)
                {
                    builder.Append($"[✓] [ ] {i}: {parameters[i].ParameterType.FullName}\n\r\t");
                }
                else
                {
                    builder.Append($"[ ] [❌] {i}: {parameters[i].ParameterType.FullName}\n\r\t");
                }
            }

            return new NullReferenceException(builder.ToString());
        }

        public void Serialize(object[] value, IPacket packetBuilder)
        {
            for (int i = 0; i < value.Length; i++)
            {
                parameters[i].AmbiguousSerialize(value[i], packetBuilder);
            }
        }

        public object[] Deserialize(IPacket packet)
        {
            int lastSucessfulDeserializationIndex = 0;
            try
            {
                object[] methodParams = new object[parameters.Length];

                for (int i = 0; i < methodParams.Length; i++)
                {
                    methodParams[i] = parameters[i].AmbiguousDeserialize(packet);
                    lastSucessfulDeserializationIndex = i;
                }

                return methodParams;
            }
            catch (IndexOutOfRangeException)
            {
                throw GenerateDetailedParameterException(lastSucessfulDeserializationIndex);
            }
        }

        public object AmbiguousDeserialize(IPacket packet) => Deserialize(packet);

        public void AmbiguousSerialize(object value, IPacket packetBuilder) => Serialize((object[])value, packetBuilder);

        public int EstimatePacketSize(object[] value)
        {
            int size = 0;
            for (int i = 0; i < value.Length; i++)
            {
                size += parameters[i].EstimatePacketSize(value[i]);
            }
            return size;
        }
    }
}
