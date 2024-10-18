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

            using (var rsa = new RSACryptoServiceProvider(1024))
            {
                _privateKey = rsa.ExportParameters(true); 
                _publicKey = rsa.ExportParameters(false); 
                _signerObject = new RSACryptoServiceProvider();
                _signerObject.ImportParameters(_privateKey);             }
        }

        public string GetUserKey()
        {
            using (var rsa = new RSACryptoServiceProvider())
            {
                rsa.ImportParameters(_publicKey);
                var publicKeyBytes = rsa.ExportCspBlob(false); 
                return BitConverter.ToString(publicKeyBytes).Replace("-", "").ToLower();
            }
        }

        public RSACryptoServiceProvider GetSignerObject()
        {
            var rsa = new RSACryptoServiceProvider();
            rsa.ImportParameters(_privateKey);
            return rsa;
        }
    }
}
