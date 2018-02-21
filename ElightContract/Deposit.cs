using System;
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Services.Neo;
using Neo.SmartContract.Framework.Services.System;
using System.Numerics;

namespace ElightContract
{
    public struct Deposit
    {
        private const Int32 kHashSize = 20;
        public const Int32 DepositSize = kHashSize << 1 + 4;
        public byte[] СarrierHash; 
        public byte[] ClientHash;
        public BigInteger Contribution;
 
        public static byte[] GetAgentAddress(Deposit deposit)
        {
            return deposit.СarrierHash.Concat(deposit.ClientHash);
        }

        public static Deposit Init(byte[] carrierHash, 
            byte[] clientHash, BigInteger contribution)
        {
            if (carrierHash.Length != kHashSize || clientHash.Length != kHashSize)
            {
                Runtime.Notify("Deposit init: Invalid parameters");
                return new Deposit();
            }

            return new Deposit
            {
                СarrierHash = carrierHash,
                ClientHash = clientHash,
                Contribution = contribution
            };
        }

        public static bool Freeze(Deposit deposit)
        {
            BigInteger balance = Token.BalanceOf(deposit.СarrierHash);

            if (deposit.Contribution > balance)
            {
                Runtime.Notify("Unable to freeze money. deposit > balance");
                return false;
            }

            byte[] agent = GetAgentAddress(deposit);
            Token.Transfer(deposit.СarrierHash, agent, deposit.Contribution);
            return true;
        }

        public static bool Unfreeze(Deposit deposit, bool isOk)
        {
            byte[] agent = GetAgentAddress(deposit);
            byte[] receiver = isOk ? deposit.СarrierHash : deposit.ClientHash;
            Token.ForceTransfer(agent, receiver);
            return true;
        }
        
        //hash is contant, so we doesn't need to allocate additional memory to store byte array length
        //[СarrierHash][ClientHash]
        //[  20 bytes ][ 20 bytes ]
        public static explicit operator byte[] (Deposit deposit)
        {
            Runtime.Notify(deposit.СarrierHash
                .Concat(deposit.ClientHash)
                .Concat(((Int32)deposit.Contribution).ToByteArray()).Length);

            return deposit.СarrierHash
                .Concat(deposit.ClientHash)
                .Concat(((Int32)deposit.Contribution).ToByteArray());
        }

        //[СarrierHash][ClientHash]
        //[  20 bytes ][ 20 bytes ]
        public static explicit operator Deposit(byte[] ba)
        {
            Runtime.Notify(ba.Length);
            return new Deposit
            {
                СarrierHash = ba.Take(kHashSize),
                ClientHash = ba.Range(kHashSize, kHashSize),
                Contribution = ba.ToInt32(kHashSize << 1, false)
            };
        }
    }
}
