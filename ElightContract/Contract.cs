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

            byte[] program = {
                0x00, 0x00, 0x00, 0x01, //1
                0x00, 0x00, 0x00, 0x02, //2
                0x80, 0x00, 0x00, 0x01, //ADD
                0x00, 0x00, 0x00, 0x10, //16
                0x80, 0x00, 0x00, 0x02, //SUB
                0x80, 0x00, 0x00, 0x00, //NEG
            };

            byte[] z = new byte[] { 0x80, 0x00, 0x00, 0x01 };
            
            Int32 a = program.ToInt32(8);
            Runtime.Notify(a);
            Runtime.Notify(a - Int32erpreter.OPCODES.ADD32);
            /*
            Int32erpreter Int32erpreter = Int32erpreter.Init();
            Int32erpreter = Int32erpreter.Run(Int32erpreter, program);

            Runtime.Notify(Int32erpreter.isOk);
            Runtime.Notify(Int32erpreter.stack.i);
            if (Int32erpreter.isOk)
            {
                Int32 res = Int32erpreter.GetResult(Int32erpreter);
                Runtime.Notify("Result ");
                Runtime.Notify(res);
                return true;
            }
            */
            return false;
        }
    }
}
