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
        public enum OPCODES
        {
            NEG  = 0x7FFFFFFF,
            SUM  = 0x7FFFFFFE,
            SUB  = 0x7FFFFFFD,
            MUL  = 0x7FFFFFFC,
            ACC  = 0x7FFFFFFB,
            CMP  = 0x7FFFFFFA,
        };
        public enum REGISTERS
        {
            ACC  = 0
        }

        public const Int32 kRegistersAmount = 1;
        public Int32[] registers;
        public Stack stack;
        public bool isOk;

        public static Interpreter Init()
        {
            Interpreter Interpreter = new Interpreter() {
                registers = new Int32[kRegistersAmount],
                stack = Stack.Init(),
                isOk = true
            };

            return Interpreter;
        }

        public static Int32 GetResult(Interpreter Interpreter)
        {
            if (Interpreter.stack.i != 0)
            {
                throw new Exception("Stack in error state");
            } 

            return Stack.Top(Interpreter.stack);
        }
        
        public static Interpreter Run(Interpreter interpreter, byte[] program, byte[] arg)
        {
            Runtime.Notify(arg.Length);
            Runtime.Notify(arg);
            if (arg.Length != 4)
            {
                interpreter.isOk = false;
                return interpreter;
            }

            interpreter.stack = Stack.Push(interpreter.stack, arg.ToInt32(0));
           
            //check
            Int32 counter = 0;
            Int32 value = 0;
            while (counter < program.Length)
            {
                value = program.ToInt32(counter);
                counter += 4;

                Int32 a = 0;
                Int32 b = 0;
                
                if (value == (Int32)OPCODES.NEG)
                {
                    a = Stack.Top(interpreter.stack);
                    interpreter.stack = Stack.Pop(interpreter.stack);
                    interpreter.stack = Stack.Push(interpreter.stack, -a);
                }
                else if (value == (Int32)OPCODES.SUM)
                {
                    a = Stack.Top(interpreter.stack);
                    interpreter.stack = Stack.Pop(interpreter.stack);
                    b = Stack.Top(interpreter.stack);
                    interpreter.stack = Stack.Pop(interpreter.stack);
                    interpreter.stack = Stack.Push(interpreter.stack, b + a);
                }
                else if (value == (Int32)OPCODES.SUB)
                {
                    a = Stack.Top(interpreter.stack);
                    interpreter.stack = Stack.Pop(interpreter.stack);
                    b = Stack.Top(interpreter.stack);
                    interpreter.stack = Stack.Pop(interpreter.stack);
                    interpreter.stack = Stack.Push(interpreter.stack, b - a);
                }
                else if (value == (Int32)OPCODES.MUL)
                {
                    a = Stack.Top(interpreter.stack);
                    interpreter.stack = Stack.Pop(interpreter.stack);
                    b = Stack.Top(interpreter.stack);
                    interpreter.stack = Stack.Pop(interpreter.stack);
                    interpreter.stack = Stack.Push(interpreter.stack, b * a);
                }
                else if (value == (Int32)OPCODES.ACC)
                {

                }
                else if (value == (Int32)OPCODES.CMP)
                {
                    a = Stack.Top(interpreter.stack);
                    interpreter.stack = Stack.Pop(interpreter.stack);
                    b = Stack.Top(interpreter.stack);
                    interpreter.stack = Stack.Pop(interpreter.stack);
                    Int32 res = b == a ? 0 : (b > a ? 1 : -1); 
                    interpreter.stack = Stack.Push(interpreter.stack, res);
                }
                else
                {
                    interpreter.stack = Stack.Push(interpreter.stack, value);
                }
            }
            
            interpreter.isOk = interpreter.stack.i == 0 & counter == program.Length;
            return interpreter;
        }
    }
}
