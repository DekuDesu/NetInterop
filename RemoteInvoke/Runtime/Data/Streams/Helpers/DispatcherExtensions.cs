using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
#nullable enable
namespace RemoteInvoke.Runtime.Data.Helpers
{
    public static class DispatcherExtensions
    {
        /// <summary>
        /// Blocking; Waits until data is received, copies the data to a new stream, Uses <paramref name="Converter"/> to create destination types
        /// <para>
        /// This object manages the lifetime of the stream, it is disposed after <paramref name="Converter"/> returns;
        /// </para>
        /// </summary>
        /// <returns></returns>
        public static T?[] DispatchPayloads<T>(this IPayloadDispatcher dispatcher, Func<Stream, T?> Converter, int count, CancellationToken token)
        {
            T?[] result = new T?[count];

            for (int i = 0; i < count; i++)
            {
                token.ThrowIfCancellationRequested();

                T? value = dispatcher.DispatchPayload(Converter, token);

                result[i] = value;
            }

            return result;
        }

        /// <summary>
        /// Blocking; Waits until data is received, copies the data to a new stream, Uses <paramref name="Converter"/> to create destination types
        /// <para>
        /// This object manages the lifetime of the stream, it is disposed after <paramref name="Converter"/> returns;
        /// </para>
        /// </summary>
        /// <returns></returns>
        public static T?[] DispatchPayloads<T>(this IPayloadDispatcher dispatcher, Func<Stream, T?> Converter, int count)
        {
            return DispatchPayloads<T>(dispatcher, Converter, count, CancellationToken.None);
        }
    }
}
