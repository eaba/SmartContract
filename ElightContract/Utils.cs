using System;

namespace ElightContract
{
    public static class Utils
    {
        public static Int32[] Copyto(this Int32[] src, Int32[] dst, Int32 index)
        {
            if (index + src.Length > dst.Length)
            {
                throw new Exception("Index is out of range!");
            }

            for (int i = 0; i < src.Length; ++ i)
            {
                dst[index + i] = src[i];
            }

            return dst;
        }
    }
}
