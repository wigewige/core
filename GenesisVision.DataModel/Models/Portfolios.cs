using System;
using System.Collections.Generic;

namespace GenesisVision.DataModel.Models
{
    public class Portfolios
    {
        public Guid InvestorAccountId { get; set; }
        public InvestorAccounts InvestorAccount { get; set; }

        public decimal Amount { get; set; }

        public ManagerTokens ManagerToken { get; set; }
        public Guid ManagerTokenId { get; set; }
    }
}
    