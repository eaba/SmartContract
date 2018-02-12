using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Services.Neo;
using System;

namespace ElightContract
{
    public static class Extensions
    {
        public static byte[] Copyto(this byte[] src, byte[] dst, Int32 index)
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

        public static byte Pop(this Stack stack)
        {
            if (stack.i < 0)
            {
                throw new Exception("Stack is empty!");
            }

            byte res = stack.arr[stack.i];
            --stack.i;
            return res;
        }

        public static Stack Push(this Stack stack, byte b)
        {
            if (stack.i + 1 >= stack.size)
            {
                //makes push 0(1) in average
                Int32 newSize = stack.size << 1;
                Stack newStack = new Stack(newSize);

                stack.arr.Copyto(newStack.arr, 0);
                
                stack.arr = newStack.arr;
                stack.size = newStack.size;
            }

            ++stack.i;
            stack.arr[stack.i] = b;
            
            return stack;
        }
    }
}
