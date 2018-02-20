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
            FAILURE,
            EXECUTION_ERROR
        }
        private static char PROGRAM_PREFIX => 'P';
        private static char PROGRAM_COUNTER_PREFIX => 'C';
        private static char PROGRAM_STATUS_PREFIX => 'S';
        private static char PROGRAM_INFO_PREFIX => 'I';

        private static string GetProgramKey(string sender, BigInteger index)
        {
            string part = PROGRAM_PREFIX + sender;
            return part + index;
        }

        private static string GetInfoKey(string sender, BigInteger index)
        {
            string infoKey = PROGRAM_INFO_PREFIX + sender;
            return infoKey + index;
        }

        private static string GetStatusKey(string sender, BigInteger index)
        {
            string infoKey = PROGRAM_STATUS_PREFIX + sender;
            return infoKey + index;
        }

        private static bool Add(string sender, byte[] program, string info)
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
            PutInfo(sender, counter, info);
            PutStatus(sender, counter, STATUS.ACTIVE);
            return true;
        }
        
        private static STATUS GetStatus(string sender, BigInteger counter)
        {
            string statusKey = GetStatusKey(sender, counter);
            byte[] status = Storage.Get(Storage.CurrentContext, statusKey);
            Runtime.Notify(status[0]);
            return (STATUS)status[0];
        }

        private static void PutStatus(string sender, BigInteger counter, STATUS status)
        {
            Runtime.Notify("PutStatus");
            Runtime.Notify(status);

            string statusKey = GetStatusKey(sender, counter);
            Runtime.Notify(statusKey);

            Storage.Delete(Storage.CurrentContext, statusKey);
            Storage.Put(Storage.CurrentContext, statusKey, (Int32)status);
        }

        private static byte[] GetInfo(string sender, BigInteger counter)
        {
            Runtime.Notify("GetInfo");
            string infoKey = GetInfoKey(sender, counter);
            byte[] info = Storage.Get(Storage.CurrentContext, infoKey);
            Runtime.Notify(info);
            return info;
        }

        private static void PutInfo(string sender, BigInteger counter, string info)
        {
            Runtime.Notify("PutInfo");
            Runtime.Notify(info);

            string infoKey = GetInfoKey(sender, counter);
            Runtime.Notify(infoKey);
            
            Storage.Put(Storage.CurrentContext, infoKey, info);
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

            STATUS status = GetStatus(sender, i);
            if (status != STATUS.ACTIVE)
            {
                Runtime.Notify("Already executeds");
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
                bool a = res == 1;
                if (a)
                {
                    PutStatus(sender, i, STATUS.SUCCESS);
                    Runtime.Notify("OK");
                } else
                {
                    PutStatus(sender, i, STATUS.FAILURE);
                    Runtime.Notify("HE OK");
                }
                return true;
            }

            PutStatus(sender, i, STATUS.EXECUTION_ERROR);
            Runtime.Notify("HE OK");

            return true;
        }

        //01 0705
        //testinvoke 0cf75529998137d1bb5baa47f9efc82852a32260 add ["AK2nJJpJr6o664CWJKi1QRXjqeic2zRp8y","000000027ffffffe7fffffff",12]
        //testinvoke 0cf75529998137d1bb5baa47f9efc82852a32260 add ["AK2nJJpJr6o664CWJKi1QRXjqeic2zRp8y","000000027ffffffe",12]
        //testinvoke 0cf75529998137d1bb5baa47f9efc82852a32260 invoke ["AK2nJJpJr6o664CWJKi1QRXjqeic2zRp8y",1,b'0000000f']  17
        //testinvoke 0cf75529998137d1bb5baa47f9efc82852a32260 invoke ["AK2nJJpJr6o664CWJKi1QRXjqeic2zRp8y",1,b'0000000f'] -17
        public static object Main(string operation, params object[] args)
        {
            Storage.Put(Storage.CurrentContext, "hello", "world");
            byte[] res = Storage.Get(Storage.CurrentContext, "hello");
            Runtime.Notify(res);
            
            if (operation == "add")
            {
                Runtime.Notify("adding program");
                return Add((string)args[0], (byte[])args[1], (string)args[2]);
            }
            else if (operation == "get")
            {
                byte[] info = GetInfo((string)args[0], (BigInteger)args[1]);
                Runtime.Notify(info);
                return true;
            }
            else if (operation == "invoke")
            {
                Runtime.Notify("Start invoking");
                Runtime.Notify(args[0]);
                Runtime.Notify(args[1]);
                return Invoke((string)args[0], (BigInteger)args[1], (byte[])args[2]);
            }
            else if (operation == "mint")
            {
                return Token.MintTokens();
            }
            else if (operation == "transfer")
            {
                string from = (string)args[0];
                string to = (string)args[1];
                BigInteger value = (BigInteger)args[2];
                return Token.Transfer(from, to, value);
            }
            else if (operation == "name")
            {
                return Token.Name();
            }
            else if (operation == "symbol")
            {
                return Token.Symbol();
            }
            else if (operation == "total")
            {
                return Token.TotalSupply();
            }
            else if (operation == "decimals")
            {
                return Token.Decimals();
            }
            
            return true;
        }
    }
}
