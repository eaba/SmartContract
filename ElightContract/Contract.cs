using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Services.Neo;
using Neo.SmartContract.Framework.Services.System;
using System;
using System.Numerics;

namespace ElightContract
{
    public class Contract : SmartContract
    {
        private static char PROGRAM_PREFIX => 'P';
        private static char PROGRAM_COUNTER_PREFIX => 'C';
        private static char PROGRAM_STATUS_PREFIX => 'S';

        private static string GetProgramKey(string senderSH, BigInteger index)
        {
            string part = PROGRAM_PREFIX + senderSH;
            return part + index;
        }

        private static bool Add(string senderSH, byte[] program)
        {
            Runtime.Notify("Start adding");
            if (!Runtime.CheckWitness(senderSH.AsByteArray()))
            {
                Runtime.Notify("Invalid witness");
                return false;
            }
            
            BigInteger counter = GetCounter(senderSH);
            counter += 1;

            Runtime.Notify("Counter");
            Runtime.Notify(counter);
            
            string key = GetProgramKey(senderSH, counter);
            Runtime.Notify("Key");
            Runtime.Notify(key);
            Storage.Put(Storage.CurrentContext, key, program);

            string counterKey = PROGRAM_COUNTER_PREFIX + senderSH;
            Runtime.Notify("Counter key");
            Runtime.Notify(counterKey);
            Storage.Put(Storage.CurrentContext, counterKey, counter);
            return true;
        }

        private static BigInteger GetCounter(string senderSH)
        {
            string counterKey = PROGRAM_COUNTER_PREFIX + senderSH;
            byte[] res = Storage.Get(Storage.CurrentContext, counterKey);
            return (res.Length == 0) ? 0 : res.AsBigInteger();
        }

        public static bool Invoke(string senderSH, BigInteger i)
        {
            if (!Runtime.CheckWitness(senderSH.AsByteArray()))
            {
                Runtime.Notify("Invalid witness");
                return false;
            }
            string key = GetProgramKey(senderSH, i);

            byte[] program = Storage.Get(Storage.CurrentContext, key);

            Runtime.Notify(program);
            if (program == null)
            {
                return false;
            }

            return true;
        }

        //01 07050505
        public static bool Main(string operation, byte[] program, byte[] arg, params object[] args)
        {
            if (operation == "addProgram")
            {
                Runtime.Notify("adding program");
                return Add((string)args[0], program);
            }
            else if (operation == "invoke")
            {
                Runtime.Notify("Start invoking");
                return Invoke((string)args[0], 1);
            }
            else if (operation == "runProgram")
            {
                Runtime.Notify("Running program");
                Interpreter interpreter = Interpreter.Init();
                interpreter = Interpreter.Run(interpreter, program, arg);

                if (interpreter.isOk)
                {
                    Int32 res = Interpreter.GetResult(interpreter);
                    Runtime.Notify("Result ");
                    Runtime.Notify(res);
                    return true;
                }
            } 
            return true;
        }
    }
}
