using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlockChain
{
    public class Block
    {
        public string Hash { get; set; }
        public string PreviousHash { get; set; }
        public List<Transaction> Transactions { get; set; }

        private string _merkleRoot;
        private long _timeStamp;
        private int _nonce;
        private string _genesisBlockPreviousHash = "0";

        public Block(string previousHash)
        {
            Transactions = new List<Transaction>();
            this.PreviousHash = previousHash;
            this._timeStamp = DateTime.Now.Ticks;
            this.Hash = CalculateHash();
        }

        public string CalculateHash()
        {
            string calculatedHash = Encryption.ApplyEncryption($"{PreviousHash}{_timeStamp}{_merkleRoot}{_nonce}");
            return calculatedHash;
        }

        public void MineBlock(int difficulty)
        {
            _merkleRoot = Encryption.GetMerkleRoot(Transactions);
            string target = new string(Enumerable.Repeat('0', difficulty).ToArray());
            while (!Hash.Substring(0, difficulty).Equals(target))
            {
                _nonce++;
                Hash = CalculateHash();
            }
        }

        public BlockChainMessageObject AddTransaction(Transaction transaction)
        {
            BlockChainMessageObject message = new BlockChainMessageObject();
            if(transaction == null)
            {
                message.Message = "Invalid Transaction!";
                message.IsValid = false;
                return message;
            }
            if(PreviousHash != _genesisBlockPreviousHash)
            {
                if(transaction.ProcessTransaction().IsValid != true)
                {
                    message.Message = "Transaction failed to process. Discarded!";
                    message.IsValid = false;
                    return message;
                }
            }
            Transactions.Add(transaction);
            message.Message = "Transaction successfully added to block";
            message.IsValid = true;
            return message;
        }
    }
}
