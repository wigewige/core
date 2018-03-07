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

        public Guid WalletId { get; set; }
        public Wallets Wallet { get; set; }

        public InvestmentPrograms InvestmentProgram { get; set; }
        public Guid? InvestmentProgramtId { get; set; }

        public PaymentTransactions PaymentTransaction { get; set; }

        public InvestmentRequests InvestmentRequest { get; set; }

        public ProfitDistributionTransactions ProfitDistributionTransaction { get; set; }
    }
}
