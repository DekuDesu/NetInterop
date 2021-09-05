using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using RemoteInvoke.Runtime.Data;

namespace RemoteInvokeTests.Runtime
{
    public class HeaderTests
    {
        [Theory]
        [InlineData("1000001000000000000001000", 1024, 8)] // random packet size
        [InlineData("1111111111111111111111111", 65535, 255)] // largest small packet
        [InlineData("1000000000000000000000000", 0, 0)] // smallest small packet
        [InlineData("11111111111111111111111111111111", 65535 * 127, 255)] // largest packet
        public void Test_Header(string headerString, int expectedSize, int expectedType)
        {
            // convert string to uint
            uint header = Convert.ToUInt32(headerString, 2);

            IHeaderParser parser = new DefaultHeader();

            int length = parser.GetPacketSize(header);

            Assert.Equal(expectedSize, length);

            int type = parser.GetPacketType(header);

            Assert.Equal(expectedType, type);
        }

        [Theory]
        [InlineData(1024, 8, "00000001000001000000000000001000")]
        [InlineData(65535, 255, "00000001111111111111111111111111")] // largest small packet
        public void Test_CreateHeader(ushort size, byte type, string expected)
        {
            IHeaderParser parser = new DefaultHeader();

            uint header = parser.CreateHeader(size, type);

            string binary = Convert.ToString(header, 2).PadLeft(32, '0');

            Assert.Equal(expected, binary);
        }

        [Theory]
        [InlineData(65535 * 127, 255, "11111111111111111111111111111111")]
        public void Test_CreateHeaderLarge(int size, byte type, string expected)
        {
            IHeaderParser parser = new DefaultHeader();

            uint header = parser.CreateLargeHeader(size, type);

            string binary = Convert.ToString(header, 2).PadLeft(32, '0');

            Assert.Equal(expected, binary);
        }

        [Theory]
        [InlineData(65535, 255)]
        [InlineData(0, 0)]
        public void Test_CreateRead(ushort expectedSize, byte expectedType)
        {
            IHeaderParser parser = new DefaultHeader();

            uint header = parser.CreateHeader(expectedSize, expectedType);

            int size = parser.GetPacketSize(header);

            int type = parser.GetPacketType(header);

            Assert.Equal(expectedType, type);

            Assert.Equal(expectedSize, size);

        }
    }
}
