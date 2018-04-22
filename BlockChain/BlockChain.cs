using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlockChain
{
    public static class BlockChain
    {
        private static List<Block> _blockChain = new List<Block>();
        private static Dictionary<string, TransactionOutput> _unspentTransactionOutputs = new Dictionary<string, TransactionOutput>();
        private static string previousHash = "0"; //Starting Hashcode

        private static int difficulty = 2;

        public static int Difficulty { get => difficulty; set => difficulty = value; }

        private static double minimumTransaction = 0.1;
        public static double MinimumTransaction { get => minimumTransaction; }

        public static void AddBlockTransaction(Transaction transaction)
        {
            Block b = new Block(previousHash);
            b.MineBlock(difficulty);
            previousHash = b.Hash;

            if(transaction != null)
            {
                b.AddTransaction(transaction);
            }

            _blockChain.Add(b);

        }

        public static Block GetBlock(int index)
        {
            if(index >= 0 && index < _blockChain.Count)
            {
                return _blockChain[index];
            }
            return null;
        }

        public static BlockChainMessageObject IsChainValid()
        {
            BlockChainMessageObject msg = new BlockChainMessageObject() {
                Message = "Valid BlockChain",
                IsValid = true
            };
            Block current;
            Block previous;
            string target = new string(Enumerable.Repeat('0', Difficulty).ToArray());
            for (int i = 1; i< _blockChain.Count; i++)
            {
                current = _blockChain[i];
                previous = _blockChain[i - 1];
                if(current.PreviousHash != previous.Hash)
                {
                    msg.Message = "Previous Hashes do not match!";
                    msg.IsValid = false;
                    return msg;
                }
                if(current.Hash != current.CalculateHash())
                {
                    msg.Message = "Current Hash does not match calculated hash!";
                    msg.IsValid = false;
                    return msg;
                }
                if (!current.Hash.Substring(0, difficulty).Equals(target))
                {
                    msg.Message = "This block hasn't been mined!";
                    msg.IsValid = false;
                }
            }
            return msg;
        }

        public static TransactionOutput GetUnspentTransactionOutputById(string id)
        {
            TransactionOutput output = null;
            _unspentTransactionOutputs.TryGetValue(id, out output);
            return output;
        }

        public static void AddUnspentTransactionOutput(string id, TransactionOutput output)
        {
            _unspentTransactionOutputs[id] = output;
        }

        public static List<TransactionOutput> GetUTXO()
        {
            return _unspentTransactionOutputs.Values.ToList();
        }

        public static void RemoveUnspentTransactionOutput(string id)
        {
            if (_unspentTransactionOutputs.ContainsKey(id))
            {
                _unspentTransactionOutputs.Remove(id);
            }
        }

        public static string Display()
        {
            var json = JsonConvert.SerializeObject(_blockChain,Formatting.Indented);
            return json.ToString();
        }
    }
}
