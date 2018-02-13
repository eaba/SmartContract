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
            Runtime.Notify(args[0]);

            // (x + 15) * 2 <= 50
            byte[] program = {
                0x00, 0x00, 0x00, 0x0F, //15
                0x7F, 0xFF, 0xFF, 0xFE, //SUM
                0x00, 0x00, 0x00, 0x02, //2
                0x7F, 0xFF, 0xFF, 0xFC, //MUL
                0x00, 0x00, 0x00, 0x32, //50
                0x7F, 0xFF, 0xFF, 0xFA, //CMP
            };

            Interpreter interpreter = Interpreter.Init();
            interpreter = Interpreter.Run(interpreter, program, (byte[])args[0]);

            if (interpreter.isOk)
            {
                Int32 res = Interpreter.GetResult(interpreter);
                Runtime.Notify("Result ");
                Runtime.Notify(res);
                return true;
            }
            
            return false;
        }
    }
}
