using System;
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Services.Neo;
using Neo.SmartContract.Framework.Services.System;
using System.Numerics;

namespace ElightContract
{
    public static class Deposit
    {
        public const char CONTRIBUTION_PREFIX = 'C';
        
        /*
        private static string GetContributionKey(string carier, string client, BigInteger elightContractId, BigInteger contribution)
        {

        }*/

        public static bool Freeze(string carier, BigInteger elightContractId, BigInteger contribution)
        {
            BigInteger balance = Token.BalanceOf(carier.AsByteArray());
            if (contribution > balance)
            {
                Runtime.Notify("deposit > balance");
                return false;
            }

            return true;
        }

        public static void Unfreeze(bool isFailure)
        {

        }
    }
}
