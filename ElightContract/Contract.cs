using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Services.Neo;
using Neo.SmartContract.Framework.Services.System;
using System;
using System.Numerics;

namespace ElightContract
{
    public class Contract : SmartContract
    {
        public static bool Invoke(string authorAddress, BigInteger i, byte[] arg)
        {
            if (!Runtime.CheckWitness(authorAddress.AsByteArray()))
            {
                Runtime.Notify("Invalid witness");
                return false;
            }

            Program program = Program.GetProgram(authorAddress, i);
            Runtime.Notify(program.Source);

            if (program.Status != Program.STATUS.ACTIVE)
            {
                Runtime.Notify("Already executed");
                return false;
            }
            
            byte[] source = program.Source;

            Interpreter interpreter = Interpreter.Init();
            interpreter = Interpreter.Run(interpreter, program, arg);

            Program.STATUS status = Program.STATUS.EXECUTION_ERROR;
            if (interpreter.isOk)
            {
                Int32 res = Interpreter.GetResult(interpreter);
                
                bool isConditionOk = res == 1;
                if (isConditionOk)
                {
                    status = Program.STATUS.SUCCESS;
                    Runtime.Notify("SUCCESS");
                }
                else
                {
                    status = Program.STATUS.FAILURE;
                    Runtime.Notify("FAILURE");
                }
            }

            Program.ChangeStatus(program, authorAddress, status);
            return status != Program.STATUS.EXECUTION_ERROR;
        }

        //05 0705
        //testinvoke script_hash add ["AK2nJJpJr6o664CWJKi1QRXjqeic2zRp8y","-26<(x+2)<26",b'000000027ffffffe0000001a7fffffff0000001a7ffffffa']
        //testinvoke script_hash invoke ["AK2nJJpJr6o664CWJKi1QRXjqeic2zRp8y",1,b'0000000f'] //true
        //testinvoke script_hash invoke ["AK2nJJpJr6o664CWJKi1QRXjqeic2zRp8y",1,b'0000001a'] //false
        public static object Main(string operation, params object[] args)
        {
            if (operation == "add")
            {
                Runtime.Notify(args[0]);
                Runtime.Notify(args[1]);
                Runtime.Notify(args[2]);

                Runtime.Notify(((byte[])args[2]).ToInt32(0));
                Runtime.Notify(((byte[])args[2]).ToInt32(4));
                Runtime.Notify(((byte[])args[2]).ToInt32(8));
                Program program = Program.Init((byte[])args[1], (byte[])args[2]);
                Program.PutProgram(program, (string)args[0]);
            }
            if (operation == "get")
            {
                return Program.GetProgram((string)args[0], (BigInteger)args[1]);
            }
            else if (operation == "invoke") 
            {
                Runtime.Notify(args[0]);
                Runtime.Notify(args[1]);
                Runtime.Notify(args[2]);
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
