using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlockChain
{
    public class TransactionInput
    {
        public string TransactionOutputId { get; set; }
        public TransactionOutput UTXO { get; set; }

        public TransactionInput(string transactionOutputId)
        {
            TransactionOutputId = transactionOutputId;
        }
    }
}
