using System;

namespace ElightContract
{
    public struct Stack
    {
        public const Int32 kDefaultSize = 1;
        public Int32 size;
        public Int32 i;
        public Int32[] arr;

        public static Stack Init(Int32 size = kDefaultSize)
        {
            Stack stack = new Stack()
            {
                i = -1,
                size = size,
                arr = new Int32[size]
            };
            return stack;
        }

        public static bool IsEmpty(Stack stack)
        {
            return stack.i == -1;
        }

        public static Int32 Top(Stack stack)
        {
            if (stack.i < 0)
            {
                throw new Exception("Stack is empty!");
            }

            return stack.arr[stack.i];
        }
        public static Stack Pop(Stack stack)
        {
            
            if (stack.i < 0)
            {
                throw new Exception("Stack is empty!");
            }

            stack.i = stack.i - 1;
            return stack;
        }

        public static Stack Push(Stack stack, Int32 v)
        {
            if (stack.i + 1 >= stack.size)
            {
                //makes push 0(1) in average
                Int32 newSize = stack.size << 1;
                Stack newStack = Init(newSize);

                stack.arr.Copyto(newStack.arr, 0);

                stack.arr = newStack.arr;
                stack.size = newStack.size;
            }

            stack.i = stack.i + 1;
            stack.arr[stack.i] = v;

            return stack;
        }
    }
}
