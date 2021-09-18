using NetInterop.Transport.Core.Packets.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetInterop.Transport.Core.Packets
{
    public class DefaultPacket<TContext> : IPacket<TContext> where TContext : Enum
    {
        private byte[] buffer;

        /// <summary>
        /// The type of packet this represents, this is enum whos value must be between 0 and 255(byte)
        /// </summary>
        public TContext PacketType { get; set; }

        /// <summary>
        /// The size of the span, does not indicate end of data within the span, use <see cref="EndOffset"/> for the location of the end of data
        /// </summary>
        public int Length => buffer.Length - HeaderSize;

        /// <summary>
        /// The actual size of the packet including the reserved space for the header
        /// </summary>
        public int ActualSize => buffer.Length;

        /// <summary>
        /// The offset from the start where appended data ends
        /// <code>
        /// -------------- ↓  
        /// </code>
        /// <code>
        /// 0, 1, 2, 3, 4, _
        /// </code>
        /// </summary>
        public int EndOffset { get; private set; }

        /// <summary>
        /// The offset from the start where unconsumed data begins
        /// <code>
        /// ----- ↓  
        /// </code>
        /// <code>
        ///              
        /// 0, 1, 2, 3, 4, _
        /// </code>
        /// </summary>
        public int StartOffset { get; private set; }

        /// <summary>
        /// The size of the reserved header stored at the beginning of the packet
        /// </summary>
        public int HeaderSize { get; set; }

        public const int DefaultHeaderSize = sizeof(uint);

        private const int MaxPacketSize = DefaultHeaderSize + ushort.MaxValue;

        public DefaultPacket(TContext packetType, byte[] data, int headerSize = DefaultHeaderSize)
        {
            PacketType = packetType;
            buffer = new byte[data.Length + headerSize];
            data.CopyTo(buffer, headerSize);
            EndOffset = buffer.Length;
            StartOffset = 0;
            HeaderSize = headerSize;
        }

        public DefaultPacket(TContext packetType, int estimatedLength, int headerSize = DefaultHeaderSize)
        {
            PacketType = packetType;
            HeaderSize = headerSize;
            buffer = new byte[estimatedLength + headerSize];
            EndOffset = 0;
            StartOffset = 0;
        }

        /// <summary>
        /// Gets a portion of the packet for manual appending of data
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public ref byte GetBuffer(int length)
        {
            // make sure we have adequet space
            Extend(length);

            int offset = EndOffset;

            EndOffset += length;

            return ref buffer[HeaderSize + offset];
        }

        private ref byte GetPointer(int index)
        {
            return ref buffer[index];
        }

        /// <summary>
        /// Extends the end of the packet to allow additional data to be appended
        /// </summary>
        /// <param name="count"></param>
        /// <returns>The index of the last element within the span, this can be used to immediately insert data to the end within re-finding the end of the span</returns>
        private void Extend(int count)
        {
            int desiredLength = EndOffset + HeaderSize + count;

            // check to see if we need to extend the span, if we dont do nothing
            // this is for when the consumer sets the backing array of the span to the estimated size before they begin appending data
            // this way we don't re-allocate new backing arrays for every change
            if (desiredLength <= Length)
            {
                return;
            }

            // current max packet size is 65535
            if (desiredLength >= MaxPacketSize)
            {
                throw new ArgumentOutOfRangeException($"Packet too large Size: {desiredLength}! Packets are limited to {ushort.MaxValue} bytes in length.");
            }

            // resize the span
            Array.Resize(ref buffer, desiredLength);
        }

        /// <summary>
        /// Appends the given data to the end of the packet, increments the position of the end of the packets data
        /// </summary>
        /// <param name="newData"></param>
        public void Append(byte[] newData)
        {
            int length = newData.Length;

            // make sure we have adequet space
            Extend(length);

            ref byte ptr = ref GetPointer(EndOffset + HeaderSize);

            ptr.Write(newData);

            EndOffset += length;
        }


        /// <summary>
        /// Reads the amount of data from the start of the packet and decrements the position of the end of the packet's data.
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public ref byte Remove(int length)
        {
            length = Math.Min(Length, Math.Max(length, 0));

            int index = StartOffset + HeaderSize;

            StartOffset += length;

            return ref GetPointer(index);
        }

        public ref byte GetHeader()
        {
            return ref buffer[0];
        }

        public void SetHeader(byte[] header)
        {
            ref byte ptr = ref buffer[0];

            ptr.Write(header);
        }

        public byte[] GetData() => buffer;
    }

    public static class Packet
    {
        public static IPacket<TContext> Create<TContext>(TContext packetType)
            where TContext : Enum, IConvertible
        {
            return new DefaultPacket<TContext>(packetType, 0);
        }

        public static IPacket<TContext> Create<TContext>(TContext packetType, byte[] data)
            where TContext : Enum, IConvertible
        {
            return new DefaultPacket<TContext>(packetType, data, DefaultPacket<TContext>.DefaultHeaderSize);
        }
        public static IPacket<TContext> Empty<TContext>(TContext packetType)
            where TContext : Enum, IConvertible
        {
            return new DefaultPacket<TContext>(packetType, Array.Empty<byte>(), sizeof(uint));
        }
        public static IPacket<TContext> Create<TContext>(TContext packetType, int estimatedSize)
            where TContext : Enum, IConvertible
        {
            return new DefaultPacket<TContext>(packetType, estimatedSize);
        }
    }
}
