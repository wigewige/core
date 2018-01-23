using GenesisVision.DataModel.Enums;
using System;

namespace GenesisVision.Core.ViewModels.Wallet
{
    public class WalletTransaction
    {
        public Guid Id { get; set; }
        public WalletTransactionsType Type { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
    }
}
