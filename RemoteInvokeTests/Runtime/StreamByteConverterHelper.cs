using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using System.Threading;
using NetInterop.Transport.Core.Packets.Extensions;

namespace RemoteInvokeTests.Runtime
{
    public class StreamByteConverterHelper
    {
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void Test_Bool(bool data)
        {
            Span<byte> stream = stackalloc byte[0];

            stream.WriteBool(data);

            Assert.Equal(sizeof(bool), stream.Length);

            Assert.Equal(data, stream.ReadBool());

            Assert.Equal(0, stream.Length);
        }

        [Theory]
        [InlineData(12)]
        [InlineData(short.MinValue)]
        [InlineData(short.MaxValue)]
        [InlineData(2313)]
        [InlineData(-4434)]
        public void Test_shortt(short data)
        {
            Span<byte> stream = stackalloc byte[0];

            stream.WriteShort(data);

            Assert.Equal(sizeof(short), stream.Length);

            Assert.Equal(data, stream.ReadShort());

            Assert.Equal(0, stream.Length);
        }

        [Theory]
        [InlineData(12)]
        [InlineData(ushort.MinValue)]
        [InlineData(ushort.MaxValue)]
        public void Test_ushort(ushort data)
        {
            Span<byte> stream = stackalloc byte[0];

            stream.WriteUShort(data);

            Assert.Equal(sizeof(ushort), stream.Length);

            Assert.Equal(data, stream.ReadUShort());

            Assert.Equal(0, stream.Length);
        }

        [Theory]
        [InlineData(12)]
        [InlineData(int.MinValue)]
        [InlineData(int.MaxValue)]
        public void Test_Int(int data)
        {
            Span<byte> stream = stackalloc byte[0];

            stream.WriteInt(data);

            Assert.Equal(sizeof(int), stream.Length);

            Assert.Equal(data, stream.ReadInt());

            Assert.Equal(0, stream.Length);
        }

        [Theory]
        [InlineData(12)]
        [InlineData(uint.MinValue)]
        [InlineData(uint.MaxValue)]
        public void Test_UInt(uint data)
        {
            Span<byte> stream = stackalloc byte[0];

            stream.WriteUInt(data);

            Assert.Equal(sizeof(uint), stream.Length);

            Assert.Equal(data, stream.ReadUInt());

            Assert.Equal(0, stream.Length);
        }

        [Theory]
        [InlineData(12)]
        [InlineData(long.MinValue)]
        [InlineData(long.MaxValue)]
        public void Test_long(long data)
        {
            Span<byte> stream = stackalloc byte[0];

            stream.WriteLong(data);

            Assert.Equal(sizeof(long), stream.Length);

            Assert.Equal(data, stream.ReadLong());

            Assert.Equal(0, stream.Length);
        }

        [Theory]
        [InlineData(12)]
        [InlineData(ulong.MinValue)]
        [InlineData(ulong.MaxValue)]
        public void Test_ulong(ulong data)
        {
            Span<byte> stream = stackalloc byte[0];

            stream.WriteULong(data);

            Assert.Equal(sizeof(ulong), stream.Length);

            Assert.Equal(data, stream.ReadULong());

            Assert.Equal(0, stream.Length);
        }

        [Theory]
        [InlineData(12.989898989898)]
        [InlineData(float.MinValue)]
        [InlineData(float.MaxValue)]
        public void Test_float(float data)
        {
            Span<byte> stream = stackalloc byte[0];

            stream.WriteFloat(data);

            Assert.Equal(sizeof(float), stream.Length);

            Assert.Equal(data, stream.ReadFloat());

            Assert.Equal(0, stream.Length);
        }

        [Theory]
        [InlineData(12)]
        [InlineData(double.MinValue)]
        [InlineData(double.MaxValue)]
        public void Test_double(double data)
        {
            Span<byte> stream = stackalloc byte[0];

            stream.WriteDouble(data);

            Assert.Equal(sizeof(double), stream.Length);

            Assert.Equal(data, stream.ReadDouble());

            Assert.Equal(0, stream.Length);
        }

        [Theory]
        [InlineData(12.19929292992929299292)]
        [InlineData(11.19929292992929299200)]
        [InlineData(0)]
        [InlineData(1)]
        public void Test_decimal(decimal data)
        {
            Span<byte> stream = stackalloc byte[0];

            stream.WriteDecimal(data);

            Assert.Equal(sizeof(decimal), stream.Length);

            Assert.Equal(data, stream.ReadDecimal());

            Assert.Equal(0, stream.Length);
        }

        [Theory]
        [InlineData("Hello World", "UTF8")]
        [InlineData("Hello World", "ASCII")]
        [InlineData("Hello World", "UTF7")]
        [InlineData("Hello World", "UTF32")]
        [InlineData("Hello World", "Latin1")]
        [InlineData("Hello World", "Unicode")]
        [InlineData("Hello World", "BigEndianUnicode")]
        [InlineData("Hello World", "Default")]
        public void Test_String(string data, string encodingName)
        {
            Encoding encoding = (Encoding)typeof(Encoding).GetProperty(encodingName, System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public).GetValue(null);

            Span<byte> stream = stackalloc byte[0];

            stream.WriteString(data, encoding);

            int expectedLength = encoding.GetByteCount("a") * data.Length;

            Assert.Equal(expectedLength, stream.Length);

            Assert.Equal(data, stream.ReadString(data.Length, encoding));

            Assert.Equal(0, stream.Length);
        }

        [Fact]
        public void TestTOSpanArray()
        {
            int[] data = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };

            Span<byte> buffer = new byte[data.Length * sizeof(int)];

            Span<byte> bytes = data.ToSpan(buffer, sizeof(int), SpanTypeConversionExtensions.ToSpan);

            Assert.Equal(bytes.Length, data.Length * sizeof(int));

            Assert.Equal(1, BitConverter.ToInt32(bytes.Slice(0, sizeof(int))));
        }
        [Fact]
        public void Test_FromSpanArray()
        {
            int[] data = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };

            Span<byte> buffer = new byte[data.Length * sizeof(int)];

            Span<byte> bytes = data.ToSpan(buffer, sizeof(int), SpanTypeConversionExtensions.ToSpan);

            Assert.Equal(bytes.Length, data.Length * sizeof(int));

            Assert.Equal(1, BitConverter.ToInt32(bytes.Slice(0, sizeof(int))));

            int[] actual = bytes.ToArray(sizeof(int), SpanTypeConversionExtensions.ToInt);

            for (int i = 0; i < data.Length; i++)
            {
                Assert.True(data[i] == actual[i], $"Expected:\t{data[i]}\r\nActual:\t{actual[i]}\r\n\tat i={i}");
            }
        }
    }
}
