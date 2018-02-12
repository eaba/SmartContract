using System;

namespace ElightContract
{
    public class Stack
    {
        public const Int32 kDefaultSize = 1;
        public Stack(Int32 size = kDefaultSize)
        {
            this.size = size;
            this.i = -1;
            this.arr = new byte[size];
        }
        
        public Int32 size;
        public Int32 i;
        public byte[] arr;
    }
}
