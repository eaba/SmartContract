using Microsoft.VisualStudio.TestTools.UnitTesting;
using ElightContract;

namespace Tests
{
    [TestClass]
    public class Int32erpreterTests
    {
        [TestMethod]
        public void Init()
        {
            Int32erpreter Int32erpreter = Int32erpreter.Init();
            Assert.AreEqual(Stack.kDefaultSize, Int32erpreter.stack.arr.Length);
            Assert.IsTrue(Stack.IsEmpty(Int32erpreter.stack));
            Assert.AreEqual(Int32erpreter.kRegistersAmount, Int32erpreter.registers32.Length);
        }

        [TestMethod]
        public void Run()
        {
            Int32erpreter Int32erpreter = Int32erpreter.Init();
            byte[] program = {
                0x00, 0x00, 0x00, 0x01, //1
                0x00, 0x00, 0x00, 0x02, //2
                0x80, 0x00, 0x00, 0x01, //ADD
                0x00, 0x00, 0x00, 0x10, //16
                0x80, 0x00, 0x00, 0x02, //SUB
                0x80, 0x00, 0x00, 0x00, //NEG
            };

            Int32erpreter = Int32erpreter.Run(Int32erpreter, program);
            Assert.IsTrue(Int32erpreter.isOk);
            Assert.AreEqual(13, Int32erpreter.GetResult(Int32erpreter));
        }
    }
}
