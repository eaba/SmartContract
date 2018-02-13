using Microsoft.VisualStudio.TestTools.UnitTesting;
using ElightContract;
using System;

namespace Tests
{
    [TestClass]
    public class UtilsTest
    {
        [TestMethod]
        public void ToInt32()
        {
            byte[] ba = new byte[] { 0x00, 0x00, 0x00, 0x01, 0x02 };
            Int32 v = ba.ToInt32(0);
            Assert.AreEqual(1, v);
            v = ba.ToInt32(1);
            Assert.AreEqual(258, v);

            ba = new byte[] { 0x00, 0x00, 0xFF, 0xFF, 0x02 };
            v = ba.ToInt32(0);
            Assert.AreEqual(65535, v);
            v = ba.ToInt32(1);
            Assert.AreEqual(16776962, v);

            ba = new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0x02 };
            v = ba.ToInt32(0);
            Assert.AreEqual(-1, v);

            ba = new byte[] { 0x80, 0x00, 0x00, 0x00, 0x01 };
            v = ba.ToInt32(0);
            Assert.AreEqual(Int32.MinValue, v);
        }
    }
}
