using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Services.Neo;
using System;
using System.Numerics;

namespace ElightContract
{
    public struct Contract
    {
        public enum STATUS
        {
            ACTIVE = 0,       //hasn't been executed yet
            SUCCESS,          //has been executed, result lays in specified borders
            FAILURE,          //has been executed, result doesn't in specified borders
            EXECUTION_ERROR   //has executed with errors
        }

        public STATUS Status;
        public byte[] Info;   //additional data, for example, author, description etc
        public byte[] Conditions; //byte code for interpreter

        private static string GetProgramKey(string authorAddress, BigInteger index)
        {
            string main = Prefixes.PROGRAM_PREFIX + authorAddress;
            return main + index;
        }

        private static string GetProgramCounterKey(string authorAddress)
        {
            return Prefixes.PROGRAM_COUNTER_PREFIX + authorAddress;
        }

        private static BigInteger GetProgramCounter(string authorAddress)
        {
            string programCounterKey = GetProgramCounterKey(authorAddress);
            byte[] programCounter = Storage.Get(Storage.CurrentContext, programCounterKey);
            return (programCounter.Length == 0) ? 0 : programCounter.AsBigInteger();
        }

        private static void PutProgramCounter(string authorAddress, BigInteger programCounter)
        {
            string programCounterKey = GetProgramCounterKey(authorAddress);
            Storage.Put(Storage.CurrentContext, programCounterKey, programCounter);
        }

        public static Contract Init(byte[] info, byte[] source)
        {
            return new Contract
            {
                Conditions = source,
                Status = STATUS.ACTIVE,
                Info = info
            };
        }

        public static Contract GetProgram(string authorAddress, BigInteger index)
        {
            string programKey = GetProgramKey(authorAddress, index);
            byte[] program = Storage.Get(Storage.CurrentContext, programKey);
            Runtime.Notify(program);
            Runtime.Notify("Contract");

            return (Contract)program;
        }

        //store program in blockchain 
        public static bool PutProgram(Contract program, string authorAddress)
        {
            Runtime.Notify("PutProgram");
            if (!Runtime.CheckWitness(authorAddress.AsByteArray()))
            {
                Runtime.Notify("Invalid witness");
                return false;
            }

            BigInteger programCounter = GetProgramCounter(authorAddress);
            programCounter += 1;
            Runtime.Notify("Counter");
            Runtime.Notify(programCounter);

            string programCounterKey = GetProgramCounterKey(authorAddress);
            string programKey = GetProgramKey(authorAddress, programCounter);
            Runtime.Notify(programCounterKey);
            Runtime.Notify(programKey);

            Storage.Put(Storage.CurrentContext, programCounterKey, programCounter);
            Storage.Put(Storage.CurrentContext, programKey, (byte[])program);
            return true;
        }
        
        private static bool UpdateProgram(Contract program, string authorAddress)
        {
            Runtime.Notify("UpdateProgram");
            if (!Runtime.CheckWitness(authorAddress.AsByteArray()))
            {
                Runtime.Notify("Invalid witness");
                return false;
            }

            BigInteger programCounter = GetProgramCounter(authorAddress);
            Runtime.Notify("Counter");
            Runtime.Notify(programCounter);

            string programCounterKey = GetProgramCounterKey(authorAddress);
            string programKey = GetProgramKey(authorAddress, programCounter);
            Runtime.Notify(programCounterKey);
            Runtime.Notify(programKey);

            Storage.Put(Storage.CurrentContext, programCounterKey, programCounter);
            return true;
        }

        //after a propgram has been executed, its status should be changed
        public static Contract ChangeStatus(Contract program, string authorAddress, STATUS status)
        {
            program.Status = status;
            BigInteger programCounter = GetProgramCounter(authorAddress);
            string programKey = GetProgramKey(authorAddress, programCounter);
            Storage.Put(Storage.CurrentContext, programKey, (byte[])program);
            return program;
        }

        //Type conversations to make possible to store Contract structure in blockchain
        //[STATUS][INFO LENGTH][INFO DATA][SRC LENGTH][SRC DATA]
        //[4 byte][  4 bytes  ][ arbitary][ 4 bytes  ][arbitary]
        public static explicit operator byte[] (Contract program)
        {
            BigInteger status = ((Int32)program.Status);
            byte[] res = ((Int32)program.Status).ToByteArray()
                .Concat(program.Info.Length.ToByteArray())
                .Concat(program.Info)
                .Concat(program.Conditions.Length.ToByteArray())
                .Concat(program.Conditions);
            
            return res;
        }

        //[STATUS][INFO LENGTH][INFO DATA][SRC LENGTH][SRC DATA]
        //[4 byte][  4 bytes  ][ arbitary][ 4 bytes  ][arbitary]
        public static explicit operator Contract(byte[] ba)
        {
            Int32 index = 0;
            STATUS status = (STATUS)ba.ToInt32(index, false);
            Runtime.Notify(status);

            index += 4;
            Int32 infoLen = ba.ToInt32(index, false);
            Runtime.Notify(infoLen);

            index += 4;
            byte[] info = ba.Range(index, infoLen);
            Runtime.Notify(info);

            index += infoLen;
            Int32 sourceLen = ba.ToInt32(index, false);
            Runtime.Notify(sourceLen);

            index += 4;
            byte[] source = ba.Range(index, sourceLen);
            Runtime.Notify(source);

            return new Contract
            {
                Status = status,
                Conditions = source,
                Info = info
            };
        }
    }
}
