using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Services.Neo;
using Neo.SmartContract.Framework.Services.System;
using System;
using System.Numerics;

namespace ElightContract
{
    public class Contract : SmartContract
    {
        private enum STATUS
        {
            ACTIVE = 0,
            SUCCESS,
            FAILURE
        }
        private static char PROGRAM_PREFIX => 'P';
        private static char PROGRAM_COUNTER_PREFIX => 'C';
        private static char PROGRAM_STATUS_PREFIX => 'S';
        private const Int32 DESCRIPTION_RESERVED = 16;

        private static string GetProgramKey(string sender, BigInteger index)
        {
            string part = PROGRAM_PREFIX + sender;
            return part + index;
        }

        private static void GetDescription(byte[] program)
        {
            Runtime.Notify(program);
            Runtime.Notify(program.Length);
            
            if (program.Length < DESCRIPTION_RESERVED)
            {
                Runtime.Notify("Invalid program format");
                return;
                //throw new Exception("Invalid pragram format");
            }

            byte[] descr = new byte[DESCRIPTION_RESERVED];
            for (int i = 0; i < DESCRIPTION_RESERVED; ++i)
            {
                descr[i] = program[i];
            }

            Runtime.Notify(descr);
        }

        private static bool Add(string sender, byte[] program)
        {
            Runtime.Notify("Start adding");
            if (!Runtime.CheckWitness(sender.AsByteArray()))
            {
                Runtime.Notify("Invalid witness");
                return false;
            }
            
            BigInteger counter = GetCounter(sender);
            counter += 1;

            Runtime.Notify("Counter");
            Runtime.Notify(counter);
            
            string key = GetProgramKey(sender, counter);
            Runtime.Notify("Key");
            Runtime.Notify(key);
            Storage.Put(Storage.CurrentContext, key, program);

            PutCounter(sender, counter);
            //PutStatus(sender, counter, STATUS.ACTIVE);
            return true;
        }

        private static void PutStatus(string sender, BigInteger counter, STATUS status)
        {
            Runtime.Notify("PutStatus");
            Runtime.Notify(status);

            string statusKey = PROGRAM_STATUS_PREFIX + sender + counter;
            Runtime.Notify(statusKey);

            Storage.Delete(Storage.CurrentContext, statusKey);
            Storage.Put(Storage.CurrentContext, statusKey, (Int32)status);
        }

        private static void GetStatus(string sender, BigInteger counter)
        {
            string statusKey = PROGRAM_STATUS_PREFIX + sender + counter;
            byte[] status = Storage.Get(Storage.CurrentContext, statusKey);
            Runtime.Notify(status);
        }

        private static void PutCounter(string sender, BigInteger counter)
        {
            string counterKey = PROGRAM_COUNTER_PREFIX + sender;
            Runtime.Notify("Counter key");
            Runtime.Notify(counterKey);
            Storage.Put(Storage.CurrentContext, counterKey, counter);
        }

        private static BigInteger GetCounter(string sender)
        {
            string counterKey = PROGRAM_COUNTER_PREFIX + sender;
            byte[] res = Storage.Get(Storage.CurrentContext, counterKey);
            return (res.Length == 0) ? 0 : res.AsBigInteger();
        }

        public static bool Invoke(string sender, BigInteger i, byte[] arg)
        {
            if (!Runtime.CheckWitness(sender.AsByteArray()))
            {
                Runtime.Notify("Invalid witness");
                return false;
            }
            string key = GetProgramKey(sender, i);

            byte[] program = Storage.Get(Storage.CurrentContext, key);

            Runtime.Notify(program);
            if (program == null)
            {
                return false;
            }
            
            Runtime.Notify(arg);
            Interpreter interpreter = Interpreter.Init();
            interpreter = Interpreter.Run(interpreter, program, arg);

            if (interpreter.isOk)
            {
                Int32 res = Interpreter.GetResult(interpreter);
                Runtime.Notify("Result ");
                Runtime.Notify(res);
                return true;
            }

            return true;
        }

        //01 0705050205
        //testinvoke 0cf75529998137d1bb5baa47f9efc82852a32260 add ["AK2nJJpJr6o664CWJKi1QRXjqeic2zRp8y","000000027ffffffe7fffffff",12]
        //testinvoke 0cf75529998137d1bb5baa47f9efc82852a32260 add ["AK2nJJpJr6o664CWJKi1QRXjqeic2zRp8y","000000027ffffffe",12]
        //testinvoke 0cf75529998137d1bb5baa47f9efc82852a32260 invoke ["AK2nJJpJr6o664CWJKi1QRXjqeic2zRp8y",1,b'0000000f']  17
        //testinvoke 0cf75529998137d1bb5baa47f9efc82852a32260 invoke ["AK2nJJpJr6o664CWJKi1QRXjqeic2zRp8y",1,b'0000000f'] -17
        public static bool Main(string operation, params object[] args)
        {
            Runtime.Notify(operation);
            Runtime.Notify(args[0]);
            Runtime.Notify(args[1]);
            Runtime.Notify(args[2]);
            Runtime.Notify(((byte[])args[2]).Length);
            
            if (operation == "check")
            {
                Runtime.Notify("check");
                if (!Runtime.CheckWitness((byte[])args[0]))
                {
                    Runtime.Notify("Invalid witness");
                    return false;
                }
                Runtime.Notify("Valid witness");
            } 
            else if (operation == "add")
            {
                Runtime.Notify("adding program");
                return Add((string)args[0], (byte[])args[1]);
            }
            else if (operation == "invoke")
            {
                Runtime.Notify("Start invoking");
                Runtime.Notify(args[0]);
                Runtime.Notify(args[1]);
                //return true;
                return Invoke((string)args[0], (BigInteger)args[1], (byte[])args[2]);
            }/*
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
            } */
            return true;
        }
    }
}
