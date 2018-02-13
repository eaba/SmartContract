using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Services.Neo;
using Neo.SmartContract.Framework.Services.System;
using System;
using System.Numerics;

namespace ElightContract
{
    public class Contract : SmartContract
    {
        public static bool Main(string operation, params object[] args)
        {
            Runtime.Notify("Main has started");
            Runtime.Notify(Interpreter.OPCODES.ADD);
            Runtime.Notify(Interpreter.OPCODES.NEG);
            Runtime.Notify(Interpreter.OPCODES.SUB);

            return true;
        }
    }
}
