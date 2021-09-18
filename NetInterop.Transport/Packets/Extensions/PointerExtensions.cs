﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetInterop.Transport.Core.Packets.Extensions
{
    // becuase unity does not support spans for what ever reason, this is the fast replacement, i dont even understand WHY they have been out for like 5 years at this fucking point so i had to make them myself
    public static unsafe class PointerExtensions
    {
        /// <summary>
        /// Copies the bytes from the privided array to the buffer
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="data"></param>
        public static void Write(this ref byte buffer, byte[] data)
        {
            fixed (byte* array = &buffer, other = data)
            {
                byte* arrayPointer = array;
                byte* otherPointer = other;

                for (int i = 0; i < data.Length; i++)
                {
                    *arrayPointer = *otherPointer;

                    arrayPointer++;
                    otherPointer++;
                }
            }
        }

        public static void Write(this ref byte buffer, byte* data, int length)
        {
            fixed (byte* array = &buffer)
            {
                byte* arrayPointer = array;
                byte* otherPointer = data;

                for (int i = 0; i < length; i++)
                {
                    *arrayPointer = *otherPointer;

                    arrayPointer++;
                    otherPointer++;
                }
            }
        }

        public static sbyte ToSByte(this ref byte buffer)
        {
            return (sbyte)buffer;
        }

        public static byte ToByte(this ref byte buffer)
        {
            return buffer;
        }

        public static void Write(this ref byte buffer, sbyte value)
        {
            buffer = (byte)value;
        }

        public static void Write(this ref byte buffer, byte value)
        {
            buffer = value;
        }

        public static bool ToBool(this ref byte buffer)
        {
            return buffer == 0xFF;
        }
        public static void Write(this ref byte buffer, bool value)
        {
            buffer = value ? (byte)0xFF : (byte)0;
        }

        public static short ToShort(this ref byte buffer)
        {
            fixed (byte* ptr = &buffer)
            {
                short* result = (short*)ptr;

                return *result;
            }
        }

        public static void Write(this ref byte buffer, short value)
        {
            fixed (byte* array = &buffer)
            {
                byte* arrayPointer = array;
                byte* numberPointer = (byte*)&value;

                *arrayPointer = *numberPointer;

                arrayPointer++;
                numberPointer++;

                *arrayPointer = *numberPointer;
            }
        }

        public static ushort ToUShort(this ref byte buffer)
        {
            fixed (byte* ptr = &buffer)
            {
                ushort* result = (ushort*)ptr;

                return *result;
            }
        }

        public static void Write(this ref byte buffer, ushort value)
        {
            fixed (byte* array = &buffer)
            {
                byte* arrayPointer = array;
                byte* numberPointer = (byte*)&value;

                *arrayPointer = *numberPointer;

                arrayPointer++;
                numberPointer++;

                *arrayPointer = *numberPointer;
            }
        }

        public static int ToInt(this ref byte buffer)
        {
            fixed (byte* ptr = &buffer)
            {
                int* result = (int*)ptr;

                return *result;
            }
        }

        public static void Write(this ref byte buffer, int value)
        {
            fixed (byte* array = &buffer)
            {
                byte* arrayPointer = array;
                byte* numberPointer = (byte*)&value;

                for (int i = 0; i < sizeof(int); i++)
                {
                    *arrayPointer = *numberPointer;

                    arrayPointer++;
                    numberPointer++;
                }
            }
        }

        public static uint ToUInt(this ref byte buffer)
        {
            fixed (byte* ptr = &buffer)
            {
                uint* result = (uint*)ptr;

                return *result;
            }
        }

        public static void Write(this ref byte buffer, uint value)
        {
            fixed (byte* array = &buffer)
            {
                byte* arrayPointer = array;
                byte* numberPointer = (byte*)&value;

                for (int i = 0; i < sizeof(uint); i++)
                {
                    *arrayPointer = *numberPointer;

                    arrayPointer++;
                    numberPointer++;
                }
            }
        }

        public static long ToLong(this ref byte buffer)
        {
            fixed (byte* ptr = &buffer)
            {
                long* result = (long*)ptr;

                return *result;
            }
        }

        public static void Write(this ref byte buffer, long value)
        {
            fixed (byte* array = &buffer)
            {
                byte* arrayPointer = array;
                byte* numberPointer = (byte*)&value;

                for (int i = 0; i < sizeof(long); i++)
                {
                    *arrayPointer = *numberPointer;

                    arrayPointer++;
                    numberPointer++;
                }
            }
        }

        public static ulong ToULong(this ref byte buffer)
        {
            fixed (byte* ptr = &buffer)
            {
                ulong* result = (ulong*)ptr;

                return *result;
            }
        }

        public static void Write(this ref byte buffer, ulong value)
        {
            fixed (byte* array = &buffer)
            {
                byte* arrayPointer = array;
                byte* numberPointer = (byte*)&value;

                for (int i = 0; i < sizeof(ulong); i++)
                {
                    *arrayPointer = *numberPointer;

                    arrayPointer++;
                    numberPointer++;
                }
            }
        }

        public static float ToFloat(this ref byte buffer)
        {
            fixed (byte* ptr = &buffer)
            {
                float* result = (float*)ptr;

                return *result;
            }
        }

        public static void Write(this ref byte buffer, float value)
        {
            fixed (byte* array = &buffer)
            {
                byte* arrayPointer = array;
                byte* numberPointer = (byte*)&value;

                for (int i = 0; i < sizeof(float); i++)
                {
                    *arrayPointer = *numberPointer;

                    arrayPointer++;
                    numberPointer++;
                }
            }
        }

        public static double ToDouble(this ref byte buffer)
        {
            fixed (byte* ptr = &buffer)
            {
                double* result = (double*)ptr;

                return *result;
            }
        }

        public static void Write(this ref byte buffer, double value)
        {
            fixed (byte* array = &buffer)
            {
                byte* arrayPointer = array;
                byte* numberPointer = (byte*)&value;

                for (int i = 0; i < sizeof(double); i++)
                {
                    *arrayPointer = *numberPointer;

                    arrayPointer++;
                    numberPointer++;
                }
            }
        }

        public static DateTime ToDateTime(this ref byte buffer)
        {
            long bits = buffer.ToLong();

            return DateTime.FromBinary(bits);
        }

        public static void Write(this ref byte buffer, DateTime value)
        {
            long bits = value.ToBinary();

            buffer.Write(bits);
        }

        public static decimal ToDecimal(this ref byte buffer)
        {
            fixed (byte* ptr = &buffer)
            {
                decimal* result = (decimal*)ptr;

                return *result;
            }
        }

        public static void Write(this ref byte buffer, decimal value)
        {
            fixed (byte* array = &buffer)
            {
                byte* arrayPointer = array;
                byte* numberPointer = (byte*)&value;

                for (int i = 0; i < sizeof(decimal); i++)
                {
                    *arrayPointer = *numberPointer;

                    arrayPointer++;
                    numberPointer++;
                }
            }
        }

        public static string ToString(this ref byte buffer, int length, Encoding encoding = default)
        {
            encoding = encoding ?? Encoding.UTF8;

            int multiplier = encoding.GetByteCount("A");

            length *= multiplier;

            fixed (byte* array = &buffer)
            {
                byte* arrayPointer = array;

                return encoding.GetString(array, length);
            }
        }

        public static void Write(this ref byte buffer, string value, Encoding encoding = default)
        {
            encoding = encoding ?? Encoding.UTF8;

            byte[] strBuffer = encoding.GetBytes(value);

            fixed (byte* array = &buffer, other = strBuffer)
            {
                byte* arrayPointer = array;
                byte* otherPointer = other;

                for (int i = 0; i < strBuffer.Length; i++)
                {
                    *arrayPointer = *otherPointer;

                    arrayPointer++;
                    otherPointer++;
                }
            }
        }
    }
}
