using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using NetInterop.Transport.Core.Abstractions.Packets;
using NetInterop.Transport.Core.Packets;
using NetInterop.Transport.Core.Packets.Extensions;
using NetInterop.Transport.Core.Factories;

namespace RemoteInvokeTests.Transport
{
    public class PacketTests
    {
        [Fact]
        public void Test_PacketInitialize()
        {
            var packet = new DefaultPacket(10);

            Assert.Equal(10, packet.Length);

            // the backing span has 4 bytes reserved for the packet header, this reduces allocations when we send the packet to the server
            // these 4 bytes are hidden from getting and setting other than using the dedicated methods
            Assert.Equal(12, packet.ActualSize);
            Assert.Equal(0, packet.EndOffset);
        }

        [Fact]
        public void Test_PacketAppend()
        {
            var packet = new DefaultPacket(10);

            Assert.Equal(10, packet.Length);
            Assert.Equal(0, packet.EndOffset);

            // when we append data the position should increment by the length of data appended
            packet.Append(new byte[] { 1, 2, 3 });

            // since the amount of data we appended was less than the array size we should verify a new buffer was not allocated
            Assert.Equal(10, packet.Length);
            Assert.Equal(3, packet.EndOffset);
            Assert.Equal(1, packet.GetData()[0 + sizeof(ushort)]);
            Assert.Equal(2, packet.GetData()[1 + sizeof(ushort)]);
            Assert.Equal(3, packet.GetData()[2 + sizeof(ushort)]);

            // append data to the END of the buffer but not exceeding make sure the buffer again does not extend
            packet.Append(new byte[] { 4, 5, 6, 7, 8, 9, 10 });

            Assert.Equal(10, packet.Length);
            Assert.Equal(10, packet.EndOffset);

            Assert.Equal(1, packet.GetData()[0 + DefaultPacket.DefaultHeaderSize]);
            Assert.Equal(2, packet.GetData()[1 + DefaultPacket.DefaultHeaderSize]);
            Assert.Equal(3, packet.GetData()[2 + DefaultPacket.DefaultHeaderSize]);
            Assert.Equal(4, packet.GetData()[3 + DefaultPacket.DefaultHeaderSize]);
            Assert.Equal(5, packet.GetData()[4 + DefaultPacket.DefaultHeaderSize]);
            Assert.Equal(6, packet.GetData()[5 + DefaultPacket.DefaultHeaderSize]);
            Assert.Equal(7, packet.GetData()[6 + DefaultPacket.DefaultHeaderSize]);
            Assert.Equal(8, packet.GetData()[7 + DefaultPacket.DefaultHeaderSize]);
            Assert.Equal(9, packet.GetData()[8 + DefaultPacket.DefaultHeaderSize]);
            Assert.Equal(10, packet.GetData()[9 + DefaultPacket.DefaultHeaderSize]);

            // append more than the backing array's size and ensure the backing array was resized and existing data was copied over
            packet.Append(new byte[] { 11 });

            Assert.Equal(11, packet.Length);
            Assert.Equal(11, packet.EndOffset);

            Assert.Equal(1, packet.GetData()[0 + DefaultPacket.DefaultHeaderSize]);
            Assert.Equal(2, packet.GetData()[1 + DefaultPacket.DefaultHeaderSize]);
            Assert.Equal(3, packet.GetData()[2 + DefaultPacket.DefaultHeaderSize]);
            Assert.Equal(4, packet.GetData()[3 + DefaultPacket.DefaultHeaderSize]);
            Assert.Equal(5, packet.GetData()[4 + DefaultPacket.DefaultHeaderSize]);
            Assert.Equal(6, packet.GetData()[5 + DefaultPacket.DefaultHeaderSize]);
            Assert.Equal(7, packet.GetData()[6 + DefaultPacket.DefaultHeaderSize]);
            Assert.Equal(8, packet.GetData()[7 + DefaultPacket.DefaultHeaderSize]);
            Assert.Equal(9, packet.GetData()[8 + DefaultPacket.DefaultHeaderSize]);
            Assert.Equal(10, packet.GetData()[9 + DefaultPacket.DefaultHeaderSize]);
            Assert.Equal(11, packet.GetData()[10 + DefaultPacket.DefaultHeaderSize]);
        }

        [Fact]
        public void Test_Remove()
        {
            var packet = new DefaultPacket(10);

            Assert.Equal(10, packet.Length);
            Assert.Equal(0, packet.EndOffset);

            // when we append data the position should increment by the length of data appended
            packet.Append(new byte[] { 1, 2, 3 });

            // make sure the values we expect exist in their intended locations
            Assert.Equal(10, packet.Length);
            Assert.Equal(3, packet.EndOffset);
            Assert.Equal(1, packet.GetData()[0 + DefaultPacket.DefaultHeaderSize]);
            Assert.Equal(2, packet.GetData()[1 + DefaultPacket.DefaultHeaderSize]);
            Assert.Equal(3, packet.GetData()[2 + DefaultPacket.DefaultHeaderSize]);

            var data = packet.Remove(3).ToArray(3);

            Assert.Equal(3, packet.StartOffset);
            Assert.Equal(1, data[0]);
            Assert.Equal(2, data[1]);
            Assert.Equal(3, data[2]);


            // make sure the old data is still there, only internal state change should have been position
            Assert.Equal(1, packet.GetData()[0 + DefaultPacket.DefaultHeaderSize]);
            Assert.Equal(2, packet.GetData()[1 + DefaultPacket.DefaultHeaderSize]);
            Assert.Equal(3, packet.GetData()[2 + DefaultPacket.DefaultHeaderSize]);
        }

        [Fact]
        public void Test_Append_Array()
        {
            var packet = new DefaultPacket(10);

            Assert.Equal(10, packet.Length);
            Assert.Equal(0, packet.EndOffset);

            int[] data = new int[] { 1, 2, 3, 4, 5 };

            packet.AppendArray(data);

            // 4 for the size, 2 for header, data * 4 for length
            Assert.Equal((data.Length * sizeof(int)) + 2 + sizeof(int), packet.ActualSize);


        }

        [Fact]
        public void Test_Get_Array()
        {
            var packet = new DefaultPacket(10);

            Assert.Equal(10, packet.Length);
            Assert.Equal(0, packet.EndOffset);

            int[] data = new int[] { 1, 2, 3, 4, 5 };

            packet.AppendArray(data);

            // 4 for the size, 2 for header, data * 4 for length
            Assert.Equal((data.Length * sizeof(int)) + 2 + sizeof(int), packet.ActualSize);

            int[] actual = packet.GetIntArray();

            for (int i = 0; i < data.Length; i++)
            {
                Assert.True(data[i] == actual[i], $"Expected:\t{data[i]}\r\nActual:\t{actual[i]}\r\n\tat i={i}");
            }
        }
    }
}
