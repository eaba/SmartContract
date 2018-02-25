using System;
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Services.Neo;
using Neo.SmartContract.Framework.Services.System;
using System.Numerics;

namespace ElightContract
{
    //represents insurance for clients in case of bad delivery
    public struct Deposit
    {
        private const Int32 kHashSize = 20;
        public const Int32 DepositSize = kHashSize << 1 + 4;
        public byte[] СarrierHash; 
        public byte[] ClientHash;
        public BigInteger Amount;

        public static Deposit Init(byte[] carrierHash, 
            byte[] clientHash, BigInteger amount)
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
                Amount = amount
            };
        }

        //freezes coins till some particular conditions are met
        public static bool Freeze(Deposit deposit)
        {
            BigInteger balance = Token.BalanceOf(deposit.СarrierHash);

            if (deposit.Amount > balance)
            {
                Runtime.Notify("Unable to freeze money. deposit > balance");
                return false;
            }
            
            Token.ForceSub(deposit.СarrierHash, deposit.Amount);
            return true;
        }
        
        public static bool Unfreeze(Deposit deposit, bool isOk)
        {
            byte[] receiver = isOk ? deposit.СarrierHash : deposit.ClientHash;
            Token.ForceAdd(receiver, deposit.Amount);
            return true;
        }
        
        //hash is constant, so we doesn't need to allocate additional memory to store byte array length
        //[СarrierHash][ClientHash]
        //[  20 bytes ][ 20 bytes ]
        public static explicit operator byte[] (Deposit deposit)
        {
            Runtime.Notify(deposit.СarrierHash
                .Concat(deposit.ClientHash)
                .Concat(((Int32)deposit.Amount).ToByteArray()).Length);

            return deposit.СarrierHash
                .Concat(deposit.ClientHash)
                .Concat(((Int32)deposit.Amount).ToByteArray());
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
                Amount = ba.ToInt32(kHashSize << 1, false)
            };
        }
    }
}
