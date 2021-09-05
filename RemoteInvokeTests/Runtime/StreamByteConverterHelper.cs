using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using RemoteInvoke.Runtime.Data.Helpers;
using System.Threading;

namespace RemoteInvokeTests.Runtime
{
    public class StreamByteConverterHelper
    {
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void Test_Bool(bool data)
        {
            using Stream s = new MemoryStream();

            s.WriteBool(data);

            s.Position = 0;

            Assert.Equal(data, s.ReadBool());
        }

        [Theory]
        [InlineData(12)]
        [InlineData(short.MinValue)]
        [InlineData(short.MaxValue)]
        [InlineData(2313)]
        [InlineData(-4434)]
        public void Test_shortt(short data)
        {
            using Stream s = new MemoryStream();

            s.WriteLong(data);

            s.Position = 0;

            Assert.Equal(data, s.ReadShort());
        }

        [Theory]
        [InlineData(12)]
        [InlineData(ushort.MinValue)]
        [InlineData(ushort.MaxValue)]
        public void Test_ushort(ushort data)
        {
            using Stream s = new MemoryStream();

            s.WriteUInt(data);

            s.Position = 0;

            Assert.Equal(data, s.ReadUShort());
        }

        [Theory]
        [InlineData(12)]
        [InlineData(int.MinValue)]
        [InlineData(int.MaxValue)]
        public void Test_Int(int data)
        {
            using Stream s = new MemoryStream();

            s.WriteLong(data);

            s.Position = 0;

            Assert.Equal(data, s.ReadInt());
        }

        [Theory]
        [InlineData(12)]
        [InlineData(uint.MinValue)]
        [InlineData(uint.MaxValue)]
        public void Test_UInt(uint data)
        {
            using Stream s = new MemoryStream();

            s.WriteUInt(data);

            s.Position = 0;

            Assert.Equal(data, s.ReadUInt());
        }

        [Theory]
        [InlineData(12)]
        [InlineData(long.MinValue)]
        [InlineData(long.MaxValue)]
        public void Test_long(long data)
        {
            using Stream s = new MemoryStream();

            s.WriteLong(data);

            s.Position = 0;

            Assert.Equal(data, s.ReadLong());
        }

        [Theory]
        [InlineData(12)]
        [InlineData(ulong.MinValue)]
        [InlineData(ulong.MaxValue)]
        public void Test_ulong(ulong data)
        {
            using Stream s = new MemoryStream();

            s.WriteULong(data);

            s.Position = 0;

            Assert.Equal(data, s.ReadULong());
        }

        [Theory]
        [InlineData(12.989898989898)]
        [InlineData(float.MinValue)]
        [InlineData(float.MaxValue)]
        public void Test_float(float data)
        {
            using Stream s = new MemoryStream();

            s.WriteFloat(data);

            s.Position = 0;

            Assert.Equal(data, s.ReadFloat());
        }

        [Theory]
        [InlineData(12)]
        [InlineData(double.MinValue)]
        [InlineData(double.MaxValue)]
        public void Test_double(double data)
        {
            using Stream s = new MemoryStream();

            s.WriteDouble(data);

            s.Position = 0;

            Assert.Equal(data, s.ReadDouble());
        }

        [Theory]
        [InlineData(12.19929292992929299292)]
        [InlineData(11.19929292992929299200)]
        [InlineData(0)]
        [InlineData(1)]
        public void Test_decimal(decimal data)
        {
            using Stream s = new MemoryStream();

            s.WriteDecimal((decimal)data);

            s.Position = 0;

            Assert.Equal(data, s.ReadDecimal());
        }

        [Fact]
        public void Test_CopyTo()
        {
            using Stream left = new MemoryStream();
            using Stream right = new MemoryStream();

            left.WriteUInt(12);

            left.Position = 0;

            left.CopyTo(right, 4, 81920, CancellationToken.None);

            right.Position = 0;

            Assert.Equal(12, right.ReadInt());
        }
    }
}
