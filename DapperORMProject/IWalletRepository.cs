using System.Collections.Generic;

namespace DapperORMProject
{
    // Interface for the Wallet Repository
    public interface IWalletRepository
    {
        int CheckIfHolderExists(string holder);
        void InsertWallet(Wallet wallet);
        void UpdateWallet(Wallet wallet);
        void DeleteWallet(int id);
        Wallet GetWalletById(int id);
        void TransferFunds(int fromId, int toId, decimal amountToTransfer);
        int InsertRecordWithIdentity(Wallet wallet);
        (decimal MinBalance, decimal MaxBalance) GetMinMaxWalletBalance();
        IEnumerable<Wallet> GetAllWallets();
    }

}
