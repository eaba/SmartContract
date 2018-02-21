using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Services.Neo;
using System;
using System.Numerics;

namespace ElightContract
{
    public struct Program
    {
        public enum STATUS
        {
            ACTIVE = 0,
            SUCCESS,
            FAILURE,
            EXECUTION_ERROR
        }

        public STATUS Status;
        public byte[] Info;
        public byte[] Source;

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

        public static Program Init(byte[] info, byte[] source)
        {
            return new Program
            {
                Source = source,
                Status = STATUS.ACTIVE,
                Info = info
            };
        }

        public static Program GetProgram(string authorAddress, BigInteger index)
        {
            string programKey = GetProgramKey(authorAddress, index);
            byte[] program = Storage.Get(Storage.CurrentContext, programKey);
            Runtime.Notify(program);
            Runtime.Notify("Program");

            return (Program)program;
        }

        public static bool PutProgram(Program program, string authorAddress)
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

        private static bool UpdateProgram(Program program, string authorAddress)
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

        public static Program ChangeStatus(Program program, string authorAddress, STATUS status)
        {
            program.Status = status;
            BigInteger programCounter = GetProgramCounter(authorAddress);
            string programKey = GetProgramKey(authorAddress, programCounter);
            Storage.Put(Storage.CurrentContext, programKey, (byte[])program);
            return program;
        }

        //[STATUS][INFO LENGTH][INFO DATA][SRC LENGTH][SRC DATA]
        //[4 byte][  4 bytes  ][ arbitary][ 4 bytes  ][arbitary]
        public static explicit operator byte[] (Program program)
        {
            BigInteger status = ((Int32)program.Status);
            byte[] res = ((Int32)program.Status).ToByteArray()
                .Concat(program.Info.Length.ToByteArray())
                .Concat(program.Info)
                .Concat(program.Source.Length.ToByteArray())
                .Concat(program.Source);
            
            return res;
        }

        //[STATUS][INFO LENGTH][INFO DATA][SRC LENGTH][SRC DATA]
        //[4 byte][  4 bytes  ][ arbitary][ 4 bytes  ][arbitary]
        public static explicit operator Program(byte[] ba)
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

            return new Program
            {
                Status = status,
                Source = source,
                Info = info
            };
        }
    }
}
