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
            NEG = Int32.MinValue,
            ADD = Int32.MinValue + 1,
            SUB = Int32.MinValue + 2
        };
        public enum REGISTERS
        {
            ACC = 0
        }

        public const Int32 kRegistersAmount = 1;
        public Int32[] registers;

        public static Interpreter Init()
        {
            Interpreter interpreter = new Interpreter() {
                registers = new Int32[kRegistersAmount]
            };

            return interpreter;
        }

        public static void Run(Interpreter interpreter)
        {
            Runtime.Notify("Start Run");
        }
    }
}
