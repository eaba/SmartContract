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
            Stack stack = new Stack();
            Assert.AreEqual(Stack.kDefaultSize, stack.size);
            Assert.AreEqual(-1, stack.i);
            Assert.AreEqual(stack.size, stack.arr.Length);


            stack = new Stack(13);
            Assert.AreEqual(13, stack.size);
            Assert.AreEqual(-1, stack.i);
            Assert.AreEqual(stack.size, stack.arr.Length);
        }

        [TestMethod]
        public void Push()
        {
            Stack stack = new Stack();

            stack.Push(0x01);
            Assert.AreEqual(1, stack.size);
            Assert.AreEqual(0, stack.i);
            Assert.AreEqual(0x01, stack.arr[0]);

            stack.Push(0x02);
            Assert.AreEqual(2, stack.size);
            Assert.AreEqual(1, stack.i);
            Assert.AreEqual(0x01, stack.arr[0]);
            Assert.AreEqual(0x02, stack.arr[1]);

            stack.Push(0x03);
            Assert.AreEqual(4, stack.size);
            Assert.AreEqual(2, stack.i);
            Assert.AreEqual(0x01, stack.arr[0]);
            Assert.AreEqual(0x02, stack.arr[1]);
            Assert.AreEqual(0x03, stack.arr[2]);

            for (int i = 0; i < 254; ++i)
            {
                stack.Push(0xFF);
            }
            Assert.AreEqual(512, stack.size);
        }

        [TestMethod]
        public void Pop()
        {
            Stack stack = new Stack();
            
            for (int i = 0; i < 17; ++i)
            {
                stack.Push((byte)i);
            }

            Assert.AreEqual(32, stack.size);
            
            for (int i = stack.i; i >= 0; --i)
            {
                byte b = stack.Pop();
                Assert.AreEqual((byte)i, b);
            }

            Assert.ThrowsException<Exception>(() => stack.Pop());
            Assert.AreEqual(32, stack.size);
            Assert.AreEqual(-1, stack.i);
        }
    }
}
