using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RemoteInvoke.Net.Transport.Packets.Extensions;

namespace RemoteInvoke.Net.Transport
{
    /// <summary>
    /// Represents a wrapper around a <see cref="Span{byte}"/> with an indentifying number for the context the <see cref="Data"/> represents, this is highly optimized to reduce memory allocations when packets are being modified and used in the pipeline
    /// </summary>
    public ref struct Packet<TContext>
        where TContext : Enum
    {
        /// <summary>
        /// The type of packet this represents, this is enum whos value must be between 0 and 255(byte)
        /// </summary>
        public TContext PacketType { get; set; }

        /// <summary>
        /// The data of the packet
        /// </summary>
        public Span<byte> Data;

        /// <summary>
        /// The size of the span, does not indicate end of data within the span, use <see cref="EndOffset"/> for the location of the end of data
        /// </summary>
        public int Length => Data.Length - HeaderSize;

        /// <summary>
        /// The actual size of the packet including the reserved space for the header
        /// </summary>
        public int ActualSize => Data.Length;

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
        public int HeaderSize { get; init; }

        public const int DefaultHeaderSize = sizeof(uint);

        private const int MaxPacketSize = DefaultHeaderSize + ushort.MaxValue;

        public Packet(TContext packetType, Span<byte> data, int headerSize = DefaultHeaderSize)
        {
            PacketType = packetType;
            Data = new byte[data.Length + headerSize];
            data.CopyTo(Data.Slice(headerSize));
            EndOffset = Data.Length;
            StartOffset = 0;
            HeaderSize = headerSize;
        }

        public Packet(TContext packetType, int estimatedLength, int headerSize = DefaultHeaderSize)
        {
            PacketType = packetType;
            HeaderSize = headerSize;
            Data = new byte[estimatedLength + headerSize];
            EndOffset = 0;
            StartOffset = 0;
        }

        /// <summary>
        /// Gets a portion of the packet for manual appending of data
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public Span<byte> GetBuffer(int length)
        {
            // make sure we have adequet space
            Extend(length);

            return Data.Slice(EndOffset++ + HeaderSize, length);
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
            Span<byte> newSpan = new byte[desiredLength];

            Data.CopyTo(newSpan);

            Data = newSpan;
        }

        /// <summary>
        /// Appends the given data to the end of the packet, increments the position of the end of the packets data
        /// </summary>
        /// <param name="newData"></param>
        public void Append(Span<byte> newData)
        {
            int length = newData.Length;

            // make sure we have adequet space
            Extend(length);

            newData.CopyTo(Data.Slice(EndOffset + HeaderSize, length));

            EndOffset += length;
        }

        /// <summary>
        /// Reads the amount of data from the start of the packet and decrements the position of the end of the packet's data.
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public Span<byte> Remove(int length)
        {
            length = Math.Min(Length, Math.Max(length, 0));

            Span<byte> result = Data.Slice(StartOffset + HeaderSize, length);

            StartOffset += length;

            return result;
        }

        /// <summary>
        /// Reads the amount of data from the end of the packet and decrements the position of the end of the packet's data.
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public Span<byte> RemoveEnd(int length)
        {
            length = Math.Min(Length, length);

            EndOffset = Math.Max(EndOffset - length, 0);

            return Data.Slice(EndOffset + HeaderSize, length);
        }

        /// <summary>
        /// Gets the reserved bytes at the start of the packet that can only be written to using <see cref="SetHeaderBytes(Span{byte})"/>
        /// </summary>
        /// <returns></returns>
        public Span<byte> GetHeaderBytes()
        {
            // we reserve space for, and hide the header bytes so we don't allocate another array when we send and receive the packet
            return Data.Slice(0, HeaderSize);
        }

        /// <summary>
        /// Sets the reserved bytes at the start of the packet that can only be read using <see cref="GetHeaderBytes"/>
        /// </summary>
        /// <param name="header"></param>
        public void SetHeaderBytes(Span<byte> header)
        {
            // we reserve space for, and hide the header bytes so we don't allocate another array when we send and receive the packet
            header.CopyTo(Data.Slice(0, HeaderSize));
        }
    }

    public static class Packet
    {
        public static Packet<TContext> Create<TContext>(TContext packetType)
            where TContext : Enum, IConvertible
        {
            return new Packet<TContext>(packetType, 0);
        }

        public static Packet<TContext> Create<TContext>(TContext packetType, Span<byte> data)
            where TContext : Enum, IConvertible
        {
            return new Packet<TContext>(packetType, data, Packet<TContext>.DefaultHeaderSize);
        }
        public static Packet<TContext> Empty<TContext>(TContext packetType)
            where TContext : Enum, IConvertible
        {
            return new Packet<TContext>()
            {
                HeaderSize = Packet<TContext>.DefaultHeaderSize,
                Data = Span<byte>.Empty,
                PacketType = packetType
            };
        }
        public static Packet<TContext> Create<TContext>(TContext packetType, int estimatedSize)
            where TContext : Enum, IConvertible
        {
            return new Packet<TContext>(packetType, estimatedSize);
        }
    }
}
