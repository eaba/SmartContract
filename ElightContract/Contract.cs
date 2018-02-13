using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Services.Neo;
using Neo.SmartContract.Framework.Services.System;
using System;
using System.Numerics;

namespace ElightContract
{
    public class Contract : SmartContract
    {
        //01 0705
        public static bool Main(string operation, params object[] args)
        {
            Runtime.Notify("Main has started");
            Runtime.Notify(Interpreter.OPCODES.ADD32);
            Runtime.Notify(Interpreter.OPCODES.NEG32);
            Runtime.Notify(Interpreter.OPCODES.SUB32);

            byte[] arr = new byte[5] { 0x00, 0x00, 0x00, 0x01, 0x02 };

            Runtime.Notify(arr[4]);
            Int32 v = arr.ToInt32(0);
            Runtime.Notify(v);
            
            return true;
        }
    }
}
