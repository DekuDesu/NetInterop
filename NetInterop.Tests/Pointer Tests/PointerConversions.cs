using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using NetInterop.Transport.Core.Packets.Extensions;

namespace RemoteInvokeTests.Pointer_Tests
{
    public class PointerConversions
    {
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void Test_Bool(bool value)
        {
            byte[] buffer = new byte[sizeof(byte)];

            ref byte ptr = ref buffer[0];

            ptr.Write(value);

            if (value)
            {
                Assert.Equal(buffer[0], (byte)0xFF);
            }
            else
            {
                Assert.Equal(buffer[0], (byte)0);
            }


            Assert.Equal(value, ptr.ToBool());
        }

        [Theory]
        [InlineData((short)0x00DE)]
        [InlineData((short)0x0DAE)]
        [InlineData(short.MinValue)]
        [InlineData(short.MaxValue)]
        [InlineData((short)0x1234)]
        [InlineData((short)0)]
        public void Test_Short(short value)
        {
            byte[] buffer = new byte[sizeof(short)];

            ref byte ptr = ref buffer[0];

            ptr.Write(value);

            Assert.Equal(value, ptr.ToShort());
        }

        [Theory]
        [InlineData((ushort)0x00DE)]
        [InlineData((ushort)0x0DAE)]
        [InlineData(ushort.MinValue)]
        [InlineData(ushort.MaxValue)]
        [InlineData((ushort)0x1234)]
        [InlineData((ushort)0xFFFA)]
        public void Test_UShort(ushort value)
        {
            byte[] buffer = new byte[sizeof(ushort)];

            ref byte ptr = ref buffer[0];

            ptr.Write(value);

            Assert.Equal(value, ptr.ToUShort());
        }

        [Theory]
        [InlineData(0x00DE)]
        [InlineData(0x0DAE)]
        [InlineData(int.MinValue)]
        [InlineData(int.MaxValue)]
        [InlineData(0x1234)]
        [InlineData(0xFFFA)]
        [InlineData(0)]
        public void Test_Int(int value)
        {
            byte[] buffer = new byte[sizeof(int)];

            ref byte ptr = ref buffer[0];

            ptr.Write(value);

            Assert.Equal(value, ptr.ToInt());
        }

        [Theory]
        [InlineData(0x00DE)]
        [InlineData(0x0DAE)]
        [InlineData(uint.MinValue)]
        [InlineData(uint.MaxValue)]
        [InlineData(0x1234)]
        [InlineData(0xFFFA)]
        public void Test_UInt(uint value)
        {
            byte[] buffer = new byte[sizeof(uint)];

            ref byte ptr = ref buffer[0];

            ptr.Write(value);

            Assert.Equal(value, ptr.ToUInt());
        }

        [Theory]
        [InlineData(0x00DE)]
        [InlineData(0x0DAE)]
        [InlineData(long.MinValue)]
        [InlineData(long.MaxValue)]
        [InlineData(0x1234)]
        [InlineData(0xFFFA)]
        public void Test_Long(long value)
        {
            byte[] buffer = new byte[sizeof(long)];

            ref byte ptr = ref buffer[0];

            ptr.Write(value);

            Assert.Equal(value, ptr.ToLong());
        }

        [Theory]
        [InlineData(0x00DE)]
        [InlineData(0x0DAE)]
        [InlineData(ulong.MinValue)]
        [InlineData(ulong.MaxValue)]
        [InlineData(0x1234)]
        [InlineData(0xFFFA)]
        public void Test_ULong(ulong value)
        {
            byte[] buffer = new byte[sizeof(ulong)];

            ref byte ptr = ref buffer[0];

            ptr.Write(value);

            Assert.Equal(value, ptr.ToULong());
        }

        [Theory]
        [InlineData(0)]
        [InlineData(0.93f)]
        [InlineData(1f)]
        [InlineData(float.MinValue)]
        [InlineData(float.MaxValue)]
        public void Test_Float(float value)
        {
            byte[] buffer = new byte[sizeof(float)];

            ref byte ptr = ref buffer[0];

            ptr.Write(value);

            Assert.Equal(value, ptr.ToFloat());
        }

        [Theory]
        [InlineData(0)]
        [InlineData(0.93f)]
        [InlineData(1f)]
        [InlineData(double.MinValue)]
        [InlineData(double.MaxValue)]
        public void Test_Double(double value)
        {
            byte[] buffer = new byte[sizeof(double)];

            ref byte ptr = ref buffer[0];

            ptr.Write(value);

            Assert.Equal(value, ptr.ToDouble());
        }



        [Theory]
        [InlineData(12.19929292992929299292)]
        [InlineData(11.19929292992929299200)]
        [InlineData(0)]
        [InlineData(1)]
        public void Test_Decimal(decimal value)
        {
            byte[] buffer = new byte[sizeof(decimal)];

            ref byte ptr = ref buffer[0];

            ptr.Write(value);

            Assert.Equal(value, ptr.ToDecimal());
        }

        [Theory]
        [InlineData("Hello World", "UTF8")]
        [InlineData("Hello World", "ASCII")]
        [InlineData("Hello World", "UTF7")]
        [InlineData("Hello World", "Latin1")]
        [InlineData("Hello World", "Default")]
        public void Test_String(string data, string encodingName)
        {
            Encoding encoding = (Encoding)typeof(Encoding).GetProperty(encodingName, System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public).GetValue(null);

            Assert.NotNull(encoding);

            byte[] expected = encoding.GetBytes(data);

            byte[] buffer = new byte[expected.Length];

            ref byte ptr = ref buffer[0];

            ptr.Write(data);

            //for (int i = 0; i < expected.Length; i++)
            //{
            //    Assert.Equal(expected[i], buffer[i]);
            //}

            string result = ptr.ToString(data.Length, encoding);

            Assert.Equal(data, result);
        }
    }
}
