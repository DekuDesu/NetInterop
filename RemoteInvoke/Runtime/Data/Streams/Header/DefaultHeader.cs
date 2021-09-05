using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemoteInvoke.Runtime.Data
{
    public class DefaultHeader : IHeaderParser
    {
        private const uint HeaderSizeTypeMask = 0x80000000;
        private const uint HeaderTypeMask = 0xFFFFFF00;

        /// <summary>
        /// Returns the number of <see langword="bytes"/> that are following the header of a given message
        /// </summary>
        /// <param name="header"></param>
        /// <returns></returns>
        public int GetPacketType(uint header)
        {
            // we use uint here becuase we use the leading bit for size type
            // int is returned becuase max message size is 127 * ushort which can fit into an int
            /*
                    HEADER SPECIFICATION
            int32
                Message Size Type       -> a  1 bit [0 - 1]   { 0 = Small, 1 = Large }
                Message Size Multiplier -> b  7 bit [0 - 127]
                    |a|b|b|b|b|b|b|b|    lowest bit

                Message Length(ushort)
                    | | | | | | | | |    mid bits
                    | | | | | | | | |    mid bits

                Unused bits(these are used for other information not related to size)
                    | | | | | | | | |    end bits

                Actual message size
                    when a = 0, Message Length
                    when a = 1, Message Length * Message Size Multiplier
            */

            // Extract the base size of the message
            // shift to the left by 8 to remove the size type and size multiplier
            uint baseSize = header << 8;

            // shift back to center and then to the right 8
            // this removes the unused bits
            baseSize >>= 16;

            // determine if it's a small message or a large
            bool largeMessage = (header & HeaderSizeTypeMask) != 0;

            if (largeMessage is false)
            {
                return (int)baseSize;
            }

            // if it's a large message the actual message size is the message size * message size multiplier

            // shift to left 1 to remove leading bit(the size type)
            uint sizeMultiplier = header << 1;

            // shift to the right 1 time to return to center, and shift 24 times to remove trailing bits
            sizeMultiplier >>= 25;

            return (int)(baseSize * sizeMultiplier);
        }

        /// <summary>
        /// Returns the identifying number for the kind of message that the header represents, this is an 8 bit number between [0 - 255] inclusive
        /// </summary>
        /// <param name="header"></param>
        /// <returns></returns>
        public int GetPacketSize(uint header)
        {
            // per the specification the header type is stored in the 8 trailing bits
            return (int)((header | HeaderTypeMask) ^ HeaderTypeMask);
        }
    }
}
