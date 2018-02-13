using Microsoft.VisualStudio.TestTools.UnitTesting;
using ElightContract;

namespace Tests
{
    [TestClass]
    public class InterpreterTests
    {
        [TestMethod]
        public void Init()
        {
            Interpreter Interpreter = Interpreter.Init();
            Assert.AreEqual(Stack.kDefaultSize, Interpreter.stack.arr.Length);
            Assert.IsTrue(Stack.IsEmpty(Interpreter.stack));
            Assert.AreEqual(Interpreter.kRegistersAmount, Interpreter.registers.Length);
        }

        [TestMethod]
        public void Run()
        {
            Interpreter interpreter = Interpreter.Init();
            byte[] arg = { 0x00, 0x00, 0x00, 0x01 };
            byte[] program = {
                0x00, 0x00, 0x00, 0x02, //2
                0x7F, 0xFF, 0xFF, 0xFE, //SUM
                0x00, 0x00, 0x00, 0x10, //16
                0x7F, 0xFF, 0xFF, 0xFD, //SUB
                0x7F, 0xFF, 0xFF, 0xFF, //NEG
                0x00, 0x00, 0x00, 0x02, //2
                0x7F, 0xFF, 0xFF, 0xFC, //MUL
                0x00, 0x00, 0x00, 0x1A, //26
                0x7F, 0xFF, 0xFF, 0xFA, //CMP
            };

            interpreter = Interpreter.Run(interpreter, program, arg);
            Assert.IsTrue(interpreter.isOk);
            Assert.AreEqual(0, Interpreter.GetResult(interpreter));
        }
    }
}
