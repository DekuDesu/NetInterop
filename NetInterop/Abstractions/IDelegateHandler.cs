using NetInterop.Transport.Core.Abstractions.Packets;
using System;

namespace NetInterop
{
    public interface IDelegateHandler
    {
        int Count { get; }

        /// <summary>
        /// Invokes, but does not remove, the delegate with the given id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="invokable"></param>
        void Invoke(ushort id);

        /// <summary>
        /// Registers the delegate and returns an id used to invoke and/or remove the delegate from the handler at a later time
        /// </summary>
        /// <param name="invokable"></param>
        /// <returns></returns>
        ushort Register<T>(T invokable) where T : Delegate;
    }
    public interface IDelegateHandler<T> : IDelegateHandler
    {
        /// <summary>
        /// Invokes, but does not remove, the delegate with the given id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="invokable"></param>
        void Invoke(ushort id, T arg);

        /// <summary>
        /// Registers the delegate and returns an id used to invoke and/or remove the delegate from the handler at a later time
        /// </summary>
        /// <param name="invokable"></param>
        /// <returns></returns>
        ushort Register(Action<T> invokable);
    }
    public interface IDelegateHandler<T1, T2> : IDelegateHandler<T1>
    {
        /// <summary>
        /// Invokes, but does not remove, the delegate with the given id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="invokable"></param>
        void Invoke(ushort id, T1 arg, T2 arg1);

        /// <summary>
        /// Registers the delegate and returns an id used to invoke and/or remove the delegate from the handler at a later time
        /// </summary>
        /// <param name="invokable"></param>
        /// <returns></returns>
        ushort Register(Action<T1, T2> invokable);
    }
}