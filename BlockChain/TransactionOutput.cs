using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlockChain
{
    public class TransactionOutput
    {
        public string Id { get; set; }
        public string RecipientPublicKey { get; set; }
        public double Value { get; set; }
        public string ParentId { get; set; }

        public TransactionOutput(string recipientPublicKey,double value, string parentId)
        {
            RecipientPublicKey = recipientPublicKey;
            Value = value;
            ParentId = parentId;
            Id = Encryption.ApplyEncryption($"{recipientPublicKey}{value}{parentId}");
        }

        public bool IsMine(string publicKey)
        {
            return publicKey == RecipientPublicKey;
        }
    }
}
