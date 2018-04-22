using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlockChain
{
    class Program
    {
        static void Main(string[] args)
        {
            Wallet walletA = new Wallet();
            Wallet walletB = new Wallet();

            Wallet coinbase = new Wallet();

            
            Transaction genesisTransaction = new Transaction(coinbase.PublicKey, walletA.PublicKey, 100, null);
            genesisTransaction.GenerateSignature(coinbase.PrivateKey);
            genesisTransaction.TransactionId = "0";
            genesisTransaction.Outputs.Add(new TransactionOutput(genesisTransaction.RecipientPublicKey, genesisTransaction.Value, genesisTransaction.TransactionId));
            BlockChain.AddUnspentTransactionOutput(genesisTransaction.Outputs[0].Id, genesisTransaction.Outputs[0]);

            Console.WriteLine("Creating and Mining Genesis block - Transferring 100 coins from coinbase to Wallet A");
            BlockChain.AddBlockTransaction(genesisTransaction);

            Console.WriteLine($"Wallet A Balance: {walletA.GetBalance()}");
            Console.WriteLine("Attempting to transfer 30 coins from Wallet A to Wallet B");
            var transaction = walletA.SendFunds(walletB.PublicKey, 30);
            if(transaction == null)
            {
                Console.WriteLine("Wallet A does not have funds to transfer");
            }
            else
            {
                BlockChain.AddBlockTransaction(transaction);
                Console.WriteLine($"Wallet A Balance: {walletA.GetBalance()}");
                Console.WriteLine($"Wallet B Balance: {walletB.GetBalance()}");
            }


            Console.WriteLine("Attempting to transfer 10 coins from Wallet B to Wallet A");
            var transaction1 = walletB.SendFunds(walletA.PublicKey, 10);
            if (transaction1 == null)
            {
                Console.WriteLine("Wallet B does not have funds to transfer");
            }
            else
            {
                BlockChain.AddBlockTransaction(transaction1);
                Console.WriteLine($"Wallet A Balance: {walletA.GetBalance()}");
                Console.WriteLine($"Wallet B Balance: {walletB.GetBalance()}");
            }

            Console.WriteLine("Check if Chain is valid");
            Console.WriteLine(BlockChain.IsChainValid());

            Console.WriteLine("BlockChain Display");
            Console.WriteLine(BlockChain.Display());
        }
    }
}
