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
            byte[] arg1 = { 0x00, 0x00, 0x00, 0x01 };
            // -26 < ((x + 2) * (-3)) < 26
            byte[] src1 = {
                0x00, 0x00, 0x00, 0x02, //2
                0x7F, 0xFF, 0xFF, 0xFE, //SUM
                0x00, 0x00, 0x00, 0x03, //3
                0x7F, 0xFF, 0xFF, 0xFF, //NEG
                0x7F, 0xFF, 0xFF, 0xFC, //MUL
                0x00, 0x00, 0x00, 0x1A, //26
                0x7F, 0xFF, 0xFF, 0xFF, //NEG
                0x00, 0x00, 0x00, 0x1A, //26
                0x7F, 0xFF, 0xFF, 0xFA, //CMP
            };
            Program program1 = new Program
            {
                Source = src1
            };

            interpreter = Interpreter.Run(interpreter, program1, arg1);
            Assert.IsTrue(interpreter.isOk);
            Assert.AreEqual(1, Interpreter.GetResult(interpreter));

            
            byte[] arg2 = { 0x00, 0x00, 0x00, 0x18 };
            // -26 < (x + 2) < 26
            byte[] src2 = {
                0x00, 0x00, 0x00, 0x02, //2
                0x7F, 0xFF, 0xFF, 0xFE, //SUM
                0x00, 0x00, 0x00, 0x1A, //26
                0x7F, 0xFF, 0xFF, 0xFF, //NEG
                0x00, 0x00, 0x00, 0x1A, //26
                0x7F, 0xFF, 0xFF, 0xFA, //CMP
            };

            Program program2 = new Program
            {
                Source = src2
            };

            interpreter = Interpreter.Init();
            interpreter = Interpreter.Run(interpreter, program2, arg2);
            Assert.IsTrue(interpreter.isOk);
            Assert.AreEqual(-1, Interpreter.GetResult(interpreter));


            byte[] arg3 = { 0x00, 0x00, 0x00, 0x3f };
            // -26 < (x + 2) < 26
            byte[] src3 = {
                0x00, 0x00, 0x00, 0x02, //2
                0x7F, 0xFF, 0xFF, 0xFE, //SUM
                0x00, 0x00, 0x00, 0x1A, //26
                0x7F, 0xFF, 0xFF, 0xFF, //NEG
                0x00, 0x00, 0x00, 0x1A, //26
                0x7F, 0xFF, 0xFF, 0xFA, //CMP
            };

            Program program3 = new Program
            {
                Source = src3
            };
            interpreter = Interpreter.Init();
            interpreter = Interpreter.Run(interpreter, program3, arg3);
            Assert.IsTrue(interpreter.isOk);
            Assert.AreEqual(-1, Interpreter.GetResult(interpreter));
            
        }
    }
}
