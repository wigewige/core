using System.Collections.Generic;

namespace GenesisVision.Core.ViewModels.Wallet
{
    public class WalletTransactionsViewModel
    {
        public List<WalletTransaction> Transactions { get; set; }

        public int Total { get; set; }
    }
}
