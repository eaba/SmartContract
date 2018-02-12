using Neo.SmartContract.Framework.Services.Neo;
using System;

namespace ElightContract
{
    public class Interpreter
    {
        //[0xF0 - 0xFF] are reserved for opcodes
        public enum OPCODES {
            NEG = 0xF0,
            ADD = 0xF1,
            SUB = 0xF2
        };
        public enum REGISTERS
        {
            ACC = 0x00000000 //accumulates result 
        };

        private static readonly Int32 kRegistersAmount = 1;
        private Stack stack;
        private Int32[] a = new Int32[kRegistersAmount];

        //forbig 
        public static void Run()
        {
            Runtime.Notify("Start Run");
            
            

            Runtime.Notify("Add {0} Sub {1}", OPCODES.ADD, OPCODES.SUB);


            Runtime.Notify("End Run");
        }
    }
}
