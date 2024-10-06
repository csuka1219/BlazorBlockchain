using BlazorBlockchain.Models;

namespace BlazorBlockchain.Services
{
    public interface IBlockchainService
    {
        void CreateTransaction(User sender, User recipient, decimal value);
        List<Transaction> GetTransactions();
        void MineBlock();
        List<Block> GetBlockChain();
    }
}
