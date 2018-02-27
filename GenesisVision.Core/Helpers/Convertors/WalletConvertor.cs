using GenesisVision.Core.ViewModels.Wallet;
using GenesisVision.DataModel.Models;

namespace GenesisVision.Core.Helpers.Convertors
{
    public static partial class Convertors
    {
        public static WalletTransaction ToTransaction(this WalletTransactions tx)
        {
            return new WalletTransaction
                   {
                       Id = tx.Id,
                       Type = tx.Type,
                       Date = tx.Date,
                       Amount = tx.Amount,
                       WalletId = tx.Wallet.Id,
                       Currency = tx.Wallet.Currency
                   };
        }
    }
}
