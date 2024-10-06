using System.Security.Cryptography;

namespace BlazorBlockchain.Models
{
    public class User
    {
        private RSAParameters _privateKey;
        public RSAParameters _publicKey;
        private RSACryptoServiceProvider _signerObject;

        public decimal Balance { get; set; }
        public string Name { get; set; }

        public User(string name, decimal balance)
        {
            Name = name;
            Balance = balance;

            // Generate RSA keys for the user (1024 bits as in Python)
            using (var rsa = new RSACryptoServiceProvider(1024))
            {
                _privateKey = rsa.ExportParameters(true);  // Export private key
                _publicKey = rsa.ExportParameters(false);  // Export public key
                _signerObject = new RSACryptoServiceProvider();
                _signerObject.ImportParameters(_privateKey);  // Load private key for signing
            }
        }

        // Get the user's public key in DER format (hex encoded)
        public string GetUserKey()
        {
            using (var rsa = new RSACryptoServiceProvider())
            {
                rsa.ImportParameters(_publicKey);
                var publicKeyBytes = rsa.ExportCspBlob(false); // Export public key in DER-like format
                return BitConverter.ToString(publicKeyBytes).Replace("-", "").ToLower();
            }
        }

        // Returns the RSA object initialized with the user's private key for signing
        public RSACryptoServiceProvider GetSignerObject()
        {
            var rsa = new RSACryptoServiceProvider();
            rsa.ImportParameters(_privateKey); // Import private key for signing
            return rsa;
        }
    }
}
