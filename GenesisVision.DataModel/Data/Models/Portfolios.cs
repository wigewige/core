using System;
using System.Collections.Generic;

namespace GenesisVision.DataModel.Models
{
    public class Portfolios
    {
        public Guid Id { get; set; }
        public ICollection<ManagerTokens> ManagerTokens { get; set; }

        public InvestorAccounts InvestorAccount { get; set; }
        public Guid InvestorAccountId { get; set; }
    }
}
