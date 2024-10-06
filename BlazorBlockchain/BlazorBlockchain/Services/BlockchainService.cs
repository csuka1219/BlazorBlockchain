using BlazorBlockchain.Models;

namespace BlazorBlockchain.Services
{
    public class BlockchainService : IBlockchainService
    {
        private List<Block> _blockChain;

        private List<Transaction> _transations;
        private List<Transaction> _pendingTransactions;

        public BlockchainService()
        {
            _blockChain = new List<Block>();
            _transations = new List<Transaction>();
            _pendingTransactions = new List<Transaction>();
        }

        private void CreateGenesisBlock(string merkleRoot)
        {           
            var genesisBlock = new Block(merkleRoot, null, 0);
            _blockChain.Add(genesisBlock);
        }

        public List<Block> GetBlockChain()
        {
            return _blockChain; 
        }

        public List<Transaction> GetTransactions()
        {
            return _transations;
        }

        public void CreateTransaction(User sender, User recipient, decimal value)
        {
            var newTransaction = new Transaction(sender, recipient, value);

            if (sender.Balance >= value)
            {
                _transations.Add(newTransaction);
                _pendingTransactions.Add(newTransaction);
                Console.WriteLine("Transaction created successfully.");
            }
            else
            {
                Console.WriteLine("Transaction failed: Insufficient balance.");
            }
        }
        public void MineBlock()
        {
            if (_pendingTransactions.Count == 0)
            {
                Console.WriteLine("No transactions to mine.");
                return;
            }

            string merkleRoot = MerkleTree.BuildMerkleTree(_pendingTransactions);

            if (_blockChain.Count == 0)
                CreateGenesisBlock(merkleRoot);
            string previousBlockHash = Block.HashBlock(_blockChain.LastOrDefault().GetBlock());

            var newBlock = new Block(merkleRoot, previousBlockHash, _blockChain.Count);

            _blockChain.Add(newBlock);

            _pendingTransactions.Clear();
        }
    }
}