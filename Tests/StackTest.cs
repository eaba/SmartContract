using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ElightContract;

namespace Tests
{
    [TestClass]
    public class StackTest
    {
        [TestMethod]
        public void Constructor()
        {
            Stack stack = Stack.Init();
            Assert.AreEqual(Stack.kDefaultSize, stack.size);
            Assert.AreEqual(-1, stack.i);
            Assert.AreEqual(stack.size, stack.arr.Length);


            stack = Stack.Init(13);
            Assert.AreEqual(13, stack.size);
            Assert.AreEqual(-1, stack.i);
            Assert.AreEqual(stack.size, stack.arr.Length);
            
        }

        [TestMethod]
        public void Push()
        {
            
            Stack stack = Stack.Init();

            stack = Stack.Push(stack, 0x01);
            Assert.AreEqual(1, stack.size);
            Assert.AreEqual(0, stack.i);
            Assert.AreEqual(0x01, stack.arr[0]);

            stack = Stack.Push(stack, 0x02);
            Assert.AreEqual(2, stack.size);
            Assert.AreEqual(1, stack.i);
            Assert.AreEqual(0x01, stack.arr[0]);
            Assert.AreEqual(0x02, stack.arr[1]);

            stack = Stack.Push(stack, 0x03);
            Assert.AreEqual(4, stack.size);
            Assert.AreEqual(2, stack.i);
            Assert.AreEqual(0x01, stack.arr[0]);
            Assert.AreEqual(0x02, stack.arr[1]);
            Assert.AreEqual(0x03, stack.arr[2]);

            for (Int32 i = 0; i < 254; ++i)
            {
                stack = Stack.Push(stack, 0xFF);
            }

            Assert.AreEqual(512, stack.size);
        }

        [TestMethod]
        public void Pop()
        {

            Stack stack = Stack.Init();

            for (Int32 i = 0; i < 17; ++i)
            {
                stack = Stack.Push(stack, i);
            }

            Assert.AreEqual(32, stack.size);
            
            for (Int32 i = stack.i; i >= 0; --i)
            {
                Int32 v = Stack.Top(stack);
                stack = Stack.Pop(stack);
                Assert.AreEqual(i, v);
            }

            Assert.ThrowsException<Exception>(() => Stack.Pop(stack));
            Assert.AreEqual(32, stack.size);
            Assert.AreEqual(-1, stack.i);
            
        }
    }
}
