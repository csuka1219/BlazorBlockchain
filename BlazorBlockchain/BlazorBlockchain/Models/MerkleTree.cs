using System.Security.Cryptography;
using System.Text;

namespace BlazorBlockchain.Models
{
    public static class MerkleTree
    {
        public static List<List<string>> MerkleTrees = new List<List<string>>();
        private static string Hash(string s)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                // Convert the transaction to string and hash it
                byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(s));
                return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
            }
        }
        public static string HashTransaction(Transaction transaction)
        {
            return Hash(transaction.GetTransactionString());
        }

        public static string BuildMerkleTree(List<Transaction> transactions)
        {
            List<string> transactionHashes = new List<string>();
            MerkleTrees.Add(new List<string>());

            foreach (var transaction in transactions)
            {
                transactionHashes.Add(HashTransaction(transaction));
                MerkleTrees.Last().Add(HashTransaction(transaction));
            }

            Console.WriteLine("---------Merkle Tree---------\n");
            while (transactionHashes.Count >= 2)
            {
                List<string> tempHashes = new List<string>();

                for (int i = 0; i < transactionHashes.Count; i += 2)
                {
                    if (i + 1 == transactionHashes.Count)
                    {
                        transactionHashes.Add(transactionHashes[i]);
                        MerkleTrees.Last().Add(transactionHashes[i]);
                    }

                    string combinedHash = Hash(transactionHashes[i] + transactionHashes[i + 1]);
                    tempHashes.Add(combinedHash);
                }

                Console.WriteLine("Current level (lower):");
                foreach (var tx in transactionHashes)
                {
                    Console.WriteLine(tx);
                }

                transactionHashes = new List<string>(tempHashes);

                Console.WriteLine("Next level (upper):");
                foreach (var tx in transactionHashes)
                {
                    MerkleTrees.Last().Add(tx);
                    Console.WriteLine(tx);
                }
            }


            return transactionHashes.FirstOrDefault();
        }
    }
}
