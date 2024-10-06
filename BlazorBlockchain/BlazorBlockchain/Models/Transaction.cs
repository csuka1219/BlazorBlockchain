using Newtonsoft.Json;
using System.Collections.Specialized;
using System.Security.Cryptography;
using System.Text;

namespace BlazorBlockchain.Models
{
    public class Transaction
    {
        public User Sender { get; }
        public User Recipient { get; }
        public decimal Value { get; }
        public string Timestamp { get; }
        public byte[] Signature { get; }

        public Transaction(User sender, User recipient, decimal value)
        {
            Sender = sender;
            Recipient = recipient;
            Value = value;
            Timestamp = DateTime.Now.ToString("yyyy/MM/dd, HH:mm:ss");

            Console.WriteLine("------------------------------------------");
            Console.WriteLine("Transaction:");
            Console.WriteLine(JsonConvert.SerializeObject(GetTransaction(), Newtonsoft.Json.Formatting.Indented));
            Signature = SignTransaction();
            Console.WriteLine($"\nSignature: {FormatSignature(Signature)}");

            try
            {
                var senderSigner = new RSACryptoServiceProvider();
                senderSigner.ImportParameters(Sender._publicKey);

                if (senderSigner.VerifyData(HashTransaction(), CryptoConfig.MapNameToOID("SHA256"), Signature))
                {
                    Console.WriteLine("The signature is valid.");
                    if (Sender.Balance >= Value)
                    {
                        Sender.Balance -= Value;
                        Recipient.Balance += Value;
                        Console.WriteLine("Success!");
                        Console.WriteLine($"{Sender.Name} successfully sent {Value} coins to {Recipient.Name}\n");
                    }
                    else
                    {
                        Console.WriteLine("Insufficient balance.");
                    }
                }
                else
                {
                    Console.WriteLine("The signature is not valid.");
                }
            }
            catch (CryptographicException)
            {
                Console.WriteLine("The signature is not valid.");
            }
        }

        private byte[] SignTransaction()
        {
            using (var signer = Sender.GetSignerObject())
            {
                var hashed = HashTransaction();
                return signer.SignData(hashed, CryptoConfig.MapNameToOID("SHA256"));
            }
        }

        private byte[] HashTransaction()
        {
            using (var sha256 = SHA256.Create())
            {
                return sha256.ComputeHash(Encoding.UTF8.GetBytes(GetTransactionString()));
            }
        }

        public string GetTransactionString()
        {
            return JsonConvert.SerializeObject(GetTransaction());
        }

        private OrderedDictionary GetTransaction()
        {
            OrderedDictionary transaction = new OrderedDictionary
        {
            { "sender", Sender.GetUserKey() },
            { "recipient", Recipient.GetUserKey() },
            { "value", Value.ToString() },
            { "time", Timestamp }
        };
            return transaction;
        }

        public static string FormatSignature(byte[] signature)
        {
            return BitConverter.ToString(signature).Replace("-", "").ToLower();
        }
    }
}
