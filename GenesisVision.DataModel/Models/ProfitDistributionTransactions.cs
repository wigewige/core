using System;
using System.Collections.Generic;
using System.Text;

namespace GenesisVision.DataModel.Models
{
    public class ProfitDistributionTransactions
    {
        public Guid Id { get; set; }

        public Guid PeriodId { get; set; }
        public Periods Period { get; set; }

        public WalletTransactions WalletTransaction { get; set; }
        public Guid WalletTransactionId { get; set; }
    }
}
