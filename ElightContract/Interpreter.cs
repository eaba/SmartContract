using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Services.Neo;
using Neo.SmartContract.Framework.Services.System;
using System;
using System.Numerics;

namespace ElightContract
{
    public struct Interpreter
    {
        //Reserved opcodes
        public enum OPCODES {
            NEG32 = Int32.MinValue,
            ADD32 = Int32.MinValue + 1,
            SUB32 = Int32.MinValue + 2
        };
        public enum REGISTERS
        {
            ACC32 = 0
        }

        public enum STATUS
        {
            ERR = 0,
            OK = 1
        }

        public const Int32 kRegistersAmount = 1;
        public Int32[] registers;
        public Stack stack;
        public STATUS status;

        public static Interpreter Init()
        {
            Interpreter interpreter = new Interpreter() {
                registers = new Int32[kRegistersAmount],
                stack = Stack.Init(),
                status = STATUS.OK
            };

            return interpreter;
        }

        public static void Run(Interpreter interpreter)
        {

        }
    }
}
