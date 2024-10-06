using System.Security.Cryptography;
using System.Text;

namespace BlazorBlockchain.Models
{
    public class Block
    {
        public string Timestamp { get; private set; }
        public string Nonce { get; private set; }
        public string MerkelRoot { get; private set; }
        public string? Hash { get; private set; }
        public string? PreviousBlockHash { get; private set; }
        public int Serial { get; private set; }

        public Block(string merkelRoot, string? prevBlockHash, int serial)
        {
            Timestamp = DateTime.Now.ToString("MM/dd/yyyy, HH:mm:ss");
            Nonce = string.Empty;
            MerkelRoot = merkelRoot;
            PreviousBlockHash = prevBlockHash;
            Serial = serial;

            FindNonce();
        }

        public Dictionary<string, object> GetBlock()
        {
            return new Dictionary<string, object>
            {
                { "timestamp", Timestamp },
                { "nonce", Nonce },
                { "merkel_root", MerkelRoot },
                { "prev_block_hash", PreviousBlockHash! },
                { "serial", Serial }
            };
        }

        private void FindNonce()
        {
            string prefix = "000";
            for (int i = 0; i < 100000; i++)
            {
                Nonce = i.ToString();
                Hash = HashBlock(GetBlock());

                if (Hash.StartsWith(prefix))
                {
                    Console.WriteLine("\n---------- Proof of Work (PoW) ----------");
                    Console.WriteLine($"Hash: {Hash}\nFound with nonce: {i}");
                    return;
                }
            }
        }

        public static string HashBlock(Dictionary<string, object> blockData)
        {
            string blockString = string.Join(",", blockData);
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(blockString));
                return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
            }
        }

        public override string ToString()
        {
            return $"Block Serial: {Serial}, " +
                   $"Timestamp: {Timestamp}, " +
                   $"Nonce: {Nonce}, " +
                   $"Merkel Root: {MerkelRoot}, " +
                   $"Previous Block Hash: {(string.IsNullOrEmpty(PreviousBlockHash) ? "None" : PreviousBlockHash)}";
        }
    }
}
