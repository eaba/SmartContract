using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Services.Neo;
using Neo.SmartContract.Framework.Services.System;
using System;
using System.Numerics;

namespace ElightContract
{
    public class Elight : SmartContract
    {
        public static bool Invoke(string carrierHash, BigInteger i, byte[] arg)
        {
            if (!Runtime.CheckWitness(carrierHash.AsByteArray()))
            {
                Runtime.Notify("Invalid witness");
                return false;
            }

            Contract contract = Contract.GetContract(carrierHash, i);
            Runtime.Notify(contract.Conditions);
            Runtime.Notify("With deposit?");
            Runtime.Notify(contract.ContractOption == Contract.Option.WithDeposit);

            if (contract.Status != Contract.STATUS.ACTIVE)
            {
                Runtime.Notify("Already executed");
                return false;
            }
            
            byte[] source = contract.Conditions;

            Interpreter interpreter = Interpreter.Init();
            interpreter = Interpreter.Run(interpreter, contract, arg);

            Contract.STATUS status = Contract.STATUS.EXECUTION_ERROR;
            if (interpreter.isOk)
            {
                Int32 res = Interpreter.GetResult(interpreter);
                
                bool isConditionOk = res == 1;
                if (isConditionOk)
                {
                    status = Contract.STATUS.SUCCESS;
                    Runtime.Notify("SUCCESS");
                }
                else
                {
                    status = Contract.STATUS.FAILURE;
                    Runtime.Notify("FAILURE");
                }

                if (contract.ContractOption == Contract.Option.WithDeposit)
                {
                    Deposit.Unfreeze(contract.Deposit, isConditionOk);
                }
            }

            Contract.ChangeStatus(contract, carrierHash, status);
            return status != Contract.STATUS.EXECUTION_ERROR;
        }


        //05 0705
        //invoke without depositing
        //testinvoke script_hash add ["AK2nJJpJr6o664CWJKi1QRXjqeic2zRp8y","-26<(x+2)<26",b'000000027ffffffe0000001a7fffffff0000001a7ffffffa']
        //testinvoke script_hash invoke ["AK2nJJpJr6o664CWJKi1QRXjqeic2zRp8y",1,b'0000000f'] //true
        //testinvoke script_hash invoke ["AK2nJJpJr6o664CWJKi1QRXjqeic2zRp8y",1,b'0000001a'] //false

        //invoke with depositing
        //testinvoke 4e3ec9a15812161d01d758ddbe3267ed19da7d88 mint[] --attach-neo=1  mint some tokens
        //init contract with depositing option
        //testinvoke bb8580f757e3aac7e15b13d53ec9adfa18980012 initDeposit ["AK2nJJpJr6o664CWJKi1QRXjqeic2zRp8y","-26<(x+2)<26",b'000000027ffffffe0000001a7fffffff0000001af7ffffffa',"AKDVzYGLczmykdtRaejgvWeZrvdkVEvQ1X",1]
        public static object Main(string operation, params object[] args)
        {
            if (operation == "init")
            {
                Contract contract = Contract.Init((byte[])args[1], (byte[])args[2]);
                Contract.PutContract(contract, (string)args[0]);
                return true;
            }
            if (operation == "initDeposit")
            {
                Contract contract = Contract.Init((byte[])args[1], (byte[])args[2]);
                contract = Contract.InitDeposit(contract, (byte[])args[0], (byte[])args[3], (BigInteger)args[4]);
                Contract.PutContract(contract, (string)args[0]);
                return true;
            }
            else if (operation == "invoke") 
            {
                return Invoke((string)args[0], (BigInteger)args[1], (byte[])args[2]);
            }
            else if (operation == "mint")
            {
                return Token.MintTokens();
            }
            else if (operation == "transfer")
            {
                return Token.Transfer((byte[])args[0], (byte[])args[1], (BigInteger)args[2]);
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
            else if (operation == "balanceOf")
            {
                return Token.BalanceOf((byte[])args[0]);
            }
            else if (operation == "decimals")
            {
                return Token.Decimals();
            }

            return false;
        }
    }
}
