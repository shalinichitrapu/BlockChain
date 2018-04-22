using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BlockChain
{
    public class Encryption
    {
        private static UnicodeEncoding _encoder = new UnicodeEncoding();

        public static string ApplyEncryption(string input)
        {
            var message = Encoding.ASCII.GetBytes(input);
            SHA256Managed hashString = new SHA256Managed();
            string hex = "";

            var hashValue = hashString.ComputeHash(message);
            foreach (byte x in hashValue)
            {
                hex += String.Format("{0:x2}", x);
            }
            return hex;
        }

        internal static bool VerifySignature(string publicKey, string data, byte[] signature)
        {
            bool success = false;
            using (var rsa = new RSACryptoServiceProvider())
            {
                byte[] originalData = _encoder.GetBytes(data);
                try
                {
                    rsa.FromXmlString(publicKey);

                    SHA512Managed Hash = new SHA512Managed();

                    byte[] hashedData = Hash.ComputeHash(signature);

                    success = rsa.VerifyData(originalData, CryptoConfig.MapNameToOID("SHA512"), signature);
                }
                catch (CryptographicException e)
                {
                    Console.WriteLine(e.Message);
                }
                finally
                {
                    rsa.PersistKeyInCsp = false;
                }
            }
            return success;
        }

        internal static byte[] ApplySignature(string privateKey, string data)
        {
            byte[] signedBytes;
            using (var rsa = new RSACryptoServiceProvider())
            {
                
                byte[] originalData = _encoder.GetBytes(data);

                try
                {
                    //// Import the private key used for signing the message
                    rsa.FromXmlString(privateKey);

                    //// Sign the data, using SHA512 as the hashing algorithm 
                    signedBytes = rsa.SignData(originalData, CryptoConfig.MapNameToOID("SHA512"));
                }
                catch (CryptographicException e)
                {
                    Console.WriteLine(e.Message);
                    return null;
                }
                finally
                {
                    //// Set the keycontainer to be cleared when rsa is garbage collected.
                    rsa.PersistKeyInCsp = false;
                }
            }
            return signedBytes;
        }

        internal static string GetMerkleRoot(List<Transaction> transactions)
        {
            int count = transactions.Count();
            List<String> previousTreeLayer = new List<String>();
            foreach(var transaction in transactions)
            {
                previousTreeLayer.Add(transaction.TransactionId);
            }
            List<String> treeLayer = previousTreeLayer;
            while (count > 1)
            {
                treeLayer = new List<String>();
                for (int i = 1; i < previousTreeLayer.Count(); i++)
                {
                    treeLayer.Add(ApplyEncryption(previousTreeLayer[i - 1] + previousTreeLayer[i]));
                }
                count = treeLayer.Count();
                previousTreeLayer = treeLayer;
            }
            String merkleRoot = (treeLayer.Count() == 1) ? treeLayer[0] : "";
            return merkleRoot;
        }
    }
}
