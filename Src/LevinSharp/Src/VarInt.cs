using System;
using AngryWasp.Helpers;

namespace LevinSharp
{
    public static class VarInt
    {
        public static byte[] To(long i)
        {
            if (i <= 63)
            {
                long j = ((i << 2) | Constants.PORTABLE_RAW_SIZE_MARK_BYTE);
                return new byte[] { (byte)j };
            }
            else if (i <= 16383)
            {
                long j = ((i << 2) | Constants.PORTABLE_RAW_SIZE_MARK_WORD);
                return BitShifter.ToByte((ushort)j);
            }
            else if (i <= 1073741823)
            {
                long j = ((i << 2) | Constants.PORTABLE_RAW_SIZE_MARK_DWORD);
                return BitShifter.ToByte((uint)j);
            }
            else
            {
                if (i > 4611686018427387903)
                    throw new Exception("Failed to pack varint - too big amount");
                
                long j = ((i << 2) | Constants.PORTABLE_RAW_SIZE_MARK_INT64);
                return BitShifter.ToByte((ulong)j);
            }
        }

        public static long From(byte[] i, ref int offset)
        {
            byte b = (byte)i[offset++];

            byte size_mask = (byte)(b & Constants.PORTABLE_RAW_SIZE_MARK_MASK);

            long v = 0;

            if (size_mask == Constants.PORTABLE_RAW_SIZE_MARK_BYTE)
                v = RightShift(b, 2);
            else if (size_mask == Constants.PORTABLE_RAW_SIZE_MARK_WORD)
                v = RightShift(ReadRemaining(i, b, 1, ref offset), 2);
            else if (size_mask == Constants.PORTABLE_RAW_SIZE_MARK_DWORD)
                v = RightShift(ReadRemaining(i, b, 3, ref offset), 2);
            else if (size_mask == Constants.PORTABLE_RAW_SIZE_MARK_INT64)
                v = RightShift(ReadRemaining(i, b, 7, ref offset), 2);

            return v;
        }

        private static long RightShift(long val, int n) => (val % 0x100000000) >> n;

        private static long ReadRemaining(byte[] i, long first, int bytes, ref int offset)
        {
            long result = first;
            for (int j = 0; j < bytes; j++)
                result += (i[offset++] << 8);

            return result;
        }
    }
}