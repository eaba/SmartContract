using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Services.Neo;
using Neo.SmartContract.Framework.Services.System;
using System;

namespace ElightContract
{
    public class Contract : SmartContract
    {
        public static bool Main(string operation, params object[] args)
        {
            Interpreter.Run();
            return true;
        }
    }
}
