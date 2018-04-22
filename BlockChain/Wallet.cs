using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BlockChain
{
    public class Wallet
    {
        public string PrivateKey { get; set; }
        public string PublicKey { get; set; }

        private Dictionary<string, TransactionOutput> _unspentTransactionOutputs;

        public Wallet()
        {
            _unspentTransactionOutputs = new Dictionary<string, TransactionOutput>();
            GenerateKeyPair();
        }

        private void GenerateKeyPair()
        {
            var rsa = new RSACryptoServiceProvider();
            PrivateKey = rsa.ToXmlString(true);
            PublicKey = rsa.ToXmlString(false);
        }

        public double GetBalance()
        {
            double total = 0;
            var UTXOs = BlockChain.GetUTXO();
            foreach(var UTXO in UTXOs)
            {
                if (UTXO.IsMine(PublicKey))
                { //if output belongs to me ( if coins belong to me )
                    _unspentTransactionOutputs[UTXO.Id] = UTXO;
                    total += UTXO.Value;
                }
            }
            return total;
        }

        public Transaction SendFunds(string recipientPublicKey, double value)
        {
            if (GetBalance() < value)
            { //gather balance and check funds.
                //System.out.println("#Not Enough funds to send transaction. Transaction Discarded.");
                return null;
            }
            //create array list of inputs
            List<TransactionInput> inputs = new List<TransactionInput>();

            double total = 0;
            foreach(var item in _unspentTransactionOutputs)
            {
                TransactionOutput UTXO = item.Value;
                total += UTXO.Value;
                inputs.Add(new TransactionInput(UTXO.Id));
                if (total > value) break;
            }

            Transaction newTransaction = new Transaction(PublicKey, recipientPublicKey, value, inputs);
            newTransaction.GenerateSignature(PrivateKey);

            foreach(var input in inputs)
            {
                _unspentTransactionOutputs.Remove(input.TransactionOutputId);
            }
            return newTransaction;
        }
    }
}
