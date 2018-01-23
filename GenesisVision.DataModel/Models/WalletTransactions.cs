using GenesisVision.DataModel.Enums;
using System;

namespace GenesisVision.DataModel.Models
{
    public class WalletTransactions
    {
        public Guid Id { get; set; }
        public WalletTransactionsType Type { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }

        public Guid UserId { get; set; }
        public Wallets Wallet { get; set; }
        public ApplicationUser User { get; set; }

        public IOTransactions IOTransaction { get; set; }

        public InvestmentRequests InvestmentRequest { get; set; }
    }
}
