using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetInterop.Helpers
{
    public static class Guids
    {
        /// <summary>
        /// Zero extends an integer to a 128 bit guid
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static unsafe Guid ToGuid(int id)
        {
            byte[] bytes = new byte[16];

            byte* numBytes = (byte*)&id;

            fixed (byte* arr = bytes)
            {
                byte* arrPtr = arr;
                byte* numPtr = numBytes;

                for (int i = 0; i < sizeof(int); i++)
                {
                    *arrPtr = *numPtr;

                    arrPtr++;
                    numPtr++;
                }
            }

            return new Guid(bytes);
        }
    }
}
