using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Services.Neo;
using Neo.SmartContract.Framework.Services.System;
using System;
using System.Numerics;

namespace ElightContract
{
    public struct Int32erpreter
    {
        //Reserved opcodes
        public enum OPCODES
        {
            NEG32 = Int32.MinValue,
            ADD32 = Int32.MinValue + 1,
            SUB32 = Int32.MinValue + 2
        };
        public enum REGISTERS
        {
            ACC32 = 0
        }

        public const Int32 kRegistersAmount = 1;
        public Int32[] registers32;
        public Stack stack;
        public bool isOk;

        public static Int32erpreter Init()
        {
            Int32erpreter Int32erpreter = new Int32erpreter() {
                registers32 = new Int32[kRegistersAmount],
                stack = Stack.Init(),
                isOk = true
            };

            return Int32erpreter;
        }

        public static Int32 GetResult(Int32erpreter Int32erpreter)
        {
            if (Int32erpreter.stack.i != 0)
            {
                throw new Exception("Stack in error state");
            } 

            return Stack.Top(Int32erpreter.stack);
        }
        
        public static Int32erpreter Run(Int32erpreter Int32erpreter, byte[] program)
        {
            //check
            Int32 counter = 0;
            Int32 value = 0;
            while (counter < program.Length)
            {
                value = program.ToInt32(counter);
                BigInteger bi = value;
                counter += 4;

                Int32 a = 0;
                Int32 b = 0;
                
                if (value == (Int32)OPCODES.NEG32)
                {
                    a = Stack.Top(Int32erpreter.stack);
                    Int32erpreter.stack = Stack.Pop(Int32erpreter.stack);
                    Int32erpreter.stack = Stack.Push(Int32erpreter.stack, -a);
                }
                else if (value == (Int32)OPCODES.ADD32)
                {
                    a = Stack.Top(Int32erpreter.stack);
                    Int32erpreter.stack = Stack.Pop(Int32erpreter.stack);
                    b = Stack.Top(Int32erpreter.stack);
                    Int32erpreter.stack = Stack.Pop(Int32erpreter.stack);
                    Int32erpreter.stack = Stack.Push(Int32erpreter.stack, b + a);
                }
                else if (value == (Int32)OPCODES.SUB32)
                {
                    a = Stack.Top(Int32erpreter.stack);
                    Int32erpreter.stack = Stack.Pop(Int32erpreter.stack);
                    b = Stack.Top(Int32erpreter.stack);
                    Int32erpreter.stack = Stack.Pop(Int32erpreter.stack);
                    Int32erpreter.stack = Stack.Push(Int32erpreter.stack, b - a);
                }
                else
                {
                    Int32erpreter.stack = Stack.Push(Int32erpreter.stack, value);
                }
            }
            
            Int32erpreter.isOk = Int32erpreter.stack.i == 0 & counter == program.Length;
            return Int32erpreter;
        }
    }
}
