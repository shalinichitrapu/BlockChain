using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlockChain
{
    public class Transaction
    {
        public string TransactionId { get; set; }
        public string SenderPublicKey { get; set; }
        public string RecipientPublicKey { get; set; }
        public double Value { get; set; }
        public byte[] Signature { get; set; }

        public List<TransactionInput> Inputs { get; set; }
        public List<TransactionOutput> Outputs { get; set; }

        private static int sequence = 0;

        private Transaction()
        {
            Outputs = new List<TransactionOutput>();
        }

        public Transaction(string fromPublicKey, string toPublicKey, double value, List<TransactionInput> inputs) : this()
        {
            SenderPublicKey = fromPublicKey;
            RecipientPublicKey = toPublicKey;
            Value = value;
            Inputs = inputs;
        }

        private string CalculateHash()
        {
            sequence++;
            return Encryption.ApplyEncryption($"{GetData}{sequence}");
        }

        public string GetData => $"{SenderPublicKey}{RecipientPublicKey}{Value}";

        public void GenerateSignature(string privateKey)
        {
            Signature = Encryption.ApplySignature(privateKey, GetData);
        }

        public bool VerifySignature()
        {
            return Encryption.VerifySignature(SenderPublicKey, GetData, Signature);
        }

        public BlockChainMessageObject ProcessTransaction()
        {
            BlockChainMessageObject message = new BlockChainMessageObject();
            if (!VerifySignature())
            {
                message.Message = "#Transaction signature failed to verify";
                message.IsValid = false;
                return message;
            }
            foreach(var transactionInput in Inputs)
            {
                transactionInput.UTXO = BlockChain.GetUnspentTransactionOutputById(transactionInput.TransactionOutputId);
            }

            var totalInputValue = GetInputsValue();
            if(totalInputValue < BlockChain.MinimumTransaction)
            {
                message.Message = $"#Transaction Input is too small {totalInputValue}";
                message.IsValid = false;
                return message;
            }

            double remainingBalance = totalInputValue - Value;
            TransactionId = CalculateHash();
            Outputs.Add(new TransactionOutput(RecipientPublicKey, Value, TransactionId));
            Outputs.Add(new TransactionOutput(SenderPublicKey, remainingBalance, TransactionId));

            foreach(var transactionOutput in Outputs)
            {
                BlockChain.AddUnspentTransactionOutput(transactionOutput.Id, transactionOutput);
            }

            foreach(var transactionInput in Inputs)
            {
                if (transactionInput.UTXO == null) continue;
                BlockChain.RemoveUnspentTransactionOutput(transactionInput.UTXO.Id);
            }
            message.Message = "Transaction Processed!";
            message.IsValid = true;
            return message;
        }

        private double GetInputsValue()
        {
            double total = 0;
            foreach(var tran in Inputs)
            {
                if (tran.UTXO == null) continue;
                total += tran.UTXO.Value;
            }
            return total;
        }

        private double GetOutputsValue()
        {
            double total = 0;
            foreach (var tran in Outputs)
            {
                total += tran.Value;
            }
            return total;
        }
    }
}
