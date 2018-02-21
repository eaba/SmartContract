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
        public const Int32 DepositSize = kHashSize << 1;
        public byte[] СarrierHash; 
        public byte[] ClientHash;
 
        public static byte[] GetAgentAddress(Deposit deposit)
        {
            return deposit.СarrierHash.Concat(deposit.ClientHash);
        }

        public static Deposit Init(byte[] carrierHash, byte[] clientHash)
        {
            if (carrierHash.Length != kHashSize || clientHash.Length != kHashSize)
            {
                Runtime.Notify("Deposit init: Invalid parameters");
                return new Deposit();
            }

            return new Deposit
            {
                СarrierHash = carrierHash,
                ClientHash = clientHash
            };
        }

        public static bool Freeze(Deposit deposit, BigInteger contribution)
        {
            BigInteger balance = Token.BalanceOf(deposit.СarrierHash);

            if (contribution > balance)
            {
                Runtime.Notify("Unable to freeze money. deposit > balance");
                return false;
            }

            byte[] agent = GetAgentAddress(deposit);
            Token.Transfer(deposit.СarrierHash, agent, contribution);
            return true;
        }

        public static bool Unfreeze(Deposit deposit, bool isFailure)
        {
            byte[] agent = GetAgentAddress(deposit);
            byte[] receiver = isFailure ? deposit.ClientHash : deposit.СarrierHash;
            Token.ForceTransfer(agent, receiver);
            return true;
        }
        
        //hash is contant, so we doesn't need to allocate additional memory to store byte array length
        //[СarrierHash][ClientHash]
        //[  20 bytes ][ 20 bytes ]
        public static explicit operator byte[] (Deposit deposit)
        {
            return deposit.СarrierHash.Concat(deposit.ClientHash);
        }

        //[СarrierHash][ClientHash]
        //[  20 bytes ][ 20 bytes ]
        public static explicit operator Deposit(byte[] ba)
        {
            return new Deposit
            {
                СarrierHash = ba.Take(kHashSize),
                ClientHash = ba.Range(kHashSize, kHashSize)
            };
        }
    }
}
