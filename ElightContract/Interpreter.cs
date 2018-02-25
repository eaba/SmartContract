using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Services.Neo;
using Neo.SmartContract.Framework.Services.System;
using System;
using System.Numerics;

namespace ElightContract
{
    //Is used to execute contracts, contracts are created dynamically, so it's 
    //important to have byte code and interpreter that's able to execute it
    public struct Interpreter
    {
        //contracts are programmed using this opcodes, interpreter then executes them
        //with arbitary parameters
        public enum OPCODES
        {
            NEG  = 0x7FFFFFFF, // MULTIPLY BY -1
            SUM  = 0x7FFFFFFE, // a, b => a + b
            SUB  = 0x7FFFFFFD, // a, b => a - b
            MUL  = 0x7FFFFFFC, // a, b => a * b 
            ACC  = 0x7FFFFFFB, // a => acc_register + a
            CMP  = 0x7FFFFFFA, // a, b, c => b < a < c 
        };

        //are planning to use in future version, when interpretator is more advanced,
        //when contract's logic is much more difficult
        public enum REGISTERS
        {
            ACC  = 0
        }

        public const Int32 kRegistersAmount = 1;
        public Int32[] registers;
        public Stack stack;
        public bool isOk;  //whether interpreter executed a contract without errors

        public static Interpreter Init()
        {
            Interpreter Interpreter = new Interpreter() {
                registers = new Int32[kRegistersAmount],
                stack = Stack.Init(),
                isOk = true
            };

            return Interpreter;
        }

        //get output value of interpretator (actually, this is last element on stack)
        public static Int32 GetResult(Interpreter Interpreter)
        {
            if (Interpreter.stack.i != 0)
            {
                throw new Exception("Stack in error state");
            } 

            return Stack.Top(Interpreter.stack);
        }
        
        public static Interpreter Run(Interpreter interpreter, Contract contract, byte[] arg)
        {
            //add arguments (it can be another program)
            contract.Conditions = arg.Concat(contract.Conditions);
            
            //run contract with specified arguments
            Int32 counter = 0;
            Int32 value = 0;
            while (counter < contract.Conditions.Length)
            {
                value = contract.Conditions.ToInt32(counter);
                counter += 4;

                Int32 a = 0;
                Int32 b = 0;
                Int32 c = 0;
                
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
                    //will implement soon
                }
                else if (value == (Int32)OPCODES.CMP)
                {
                    a = Stack.Top(interpreter.stack);
                    interpreter.stack = Stack.Pop(interpreter.stack);
                    b = Stack.Top(interpreter.stack);
                    interpreter.stack = Stack.Pop(interpreter.stack);
                    c = Stack.Top(interpreter.stack);
                    interpreter.stack = Stack.Pop(interpreter.stack);
                    Int32 res = b < c && c < a ? 1 : -1; 
                    interpreter.stack = Stack.Push(interpreter.stack, res);
                }
                else
                {
                    interpreter.stack = Stack.Push(interpreter.stack, value);
                }
            }
            
            //if all the program has been read and there is only one element on stack, then interpretator
            //executed the program correctly
            interpreter.isOk = interpreter.stack.i == 0 & counter == contract.Conditions.Length;
            return interpreter;
        }
    }
}
