using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Services.Neo;
using System;

namespace ElightContract
{
    public static class Utils
    {
        public static Int32 ToInt32(this byte[] src, Int32 index)
        {
            if (index < 0 || index + 4 > src.Length)
            {
                throw new Exception("Index is out of range!");
            }

            Int32 res = 0x00000000;
            res += (src[index + 3] & 0xFF);
            res += (src[index + 2] & 0xFF) << 8;
            res += (src[index + 1] & 0xFF) << 16;
            res += (src[index + 0] & 0xFF) << 24;
            
            return res;
        }

        public static Int32[] Copyto(this Int32[] src, Int32[] dst, Int32 index)
        {
            if (index + src.Length > dst.Length)
            {
                throw new Exception("Index is out of range!");
            }

            for (Int32 i = 0; i < src.Length; ++ i)
            {
                dst[index + i] = src[i];
            }

            return dst;
        }
    }
}
