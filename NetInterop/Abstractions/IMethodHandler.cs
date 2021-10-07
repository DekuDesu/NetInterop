using NetInterop.Transport.Core.Abstractions.Packets;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NetInterop
{
    public interface IMethodHandler
    {
        /// <summary>
        /// Registers the provided <see cref="Delegate"/> with the handler so it can be invoked on the remote client
        /// </summary>
        /// <param name="method">The <see cref="Delegate"/> that should be invoked using the <see cref="INetPtr"/></param>
        /// <returns><see cref="INetPtr"/> that can be used to invoke the method</returns>
        INetPtr Register(Delegate method);
        /// <summary>
        /// Registers the provided <see cref="Delegate"/> with the handler so it can be invoked on the remote client
        /// </summary>
        /// <typeparam name="TResult">The expected result of invoking the <see cref="Delegate"/></typeparam>
        /// <param name="method">The <see cref="Delegate"/> that should be invoked using the <see cref="INetPtr"/></param>
        /// <returns><see cref="INetPtr"/> that can be used to invoke the method</returns>
        INetPtr<TResult> Register<TResult>(Delegate method);


        /// <summary>
        /// Registers the provided <see cref="MethodInfo"/> with the handler so it can be invoked on the remote client
        /// </summary>
        /// <param name="method">The <see cref="MethodInfo"/> that should be invoked using the <see cref="INetPtr"/></param>
        /// <returns><see cref="INetPtr"/> that can be used to invoke the method</returns>
        INetPtr Register(MethodInfo method);

        /// <summary>
        /// Registers the provided <see cref="MethodInfo"/> with the handler so it can be invoked on the remote client
        /// </summary>
        /// <typeparam name="TResult">The expected result of invoking the <see cref="MethodInfo"/></typeparam>
        /// <param name="method">The <see cref="MethodInfo"/> that should be invoked using the <see cref="INetPtr"/></param>
        /// <returns><see cref="INetPtr"/> that can be used to invoke the method</returns>
        INetPtr<TResult> Register<TResult>(MethodInfo method);

        /// <summary>
        /// Invokes the provided method pointer on the local client using the provided packet to obtain parameters (if applicable) to invoke the method
        /// </summary>
        /// <param name="methodPtr">The <see cref="INetPtr"/> that points to the method to be invoked</param>
        /// <param name="packet">The packet that may or may not contain serialized parameters to invoke the method</param>
        void Invoke(INetPtr methodPtr, IPacket packet);
        /// <summary>
        /// Invokes the provided method pointer on the local client using the provided packet to obtain parameters (if applicable) to invoke the method. When the method is invoked an returned values are automatically serialized and appended to the provided packetBuilder
        /// </summary>
        /// <param name="methodPtr">The <see cref="INetPtr"/> that points to the method to be invoked</param>
        /// <param name="packet">The packet that may or may not contain serialized parameters to invoke the method</param>
        /// <param name="packetBuilder">The <see cref="IPacket"/> that should be used to append the serialized result of invoking the method</param>
        void Invoke(INetPtr methodPtr, IPacket packet, IPacket packetBuilder);
        /// <summary>
        /// Attempts to find and return a <see cref="IPacketSerializer"/> that can be used to serialize the parameters for the method whos pointer is provided.
        /// </summary>
        /// <param name="methodPtr">The pointer to the method whos parameters need to be serialized</param>
        /// <param name="serializer">The serialier that was found</param>
        /// <returns><see langword="true"/> when the serializer was found and assigned</returns>
        bool TryGetSerializer(INetPtr methodPtr, out IPacketSerializer<object[]> serializer);
        /// <summary>
        /// Attempts to find and return a <see cref="IPacketDeserializer"/> that can be used to deserialize the parameters for the method whos pointer is provided.
        /// </summary>
        /// <param name="methodPtr">The pointer to the method whos parameters need to be deserialized</param>
        /// <param name="deserializer">The eserialier that was found</param>
        /// <returns><see langword="true"/> when the deserializer was found and assigned</returns>
        bool TryGetDeserializer(INetPtr ptr, out IPacketDeserializer<object[]> deserializer);
        /// <summary>
        /// Clears all registered methods and delegates from the handler
        /// </summary>
        void Clear();
    }
}
