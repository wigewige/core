using System;
using System.Collections.Generic;

namespace GenesisVision.DataModel.Models
{
    public class InvestorAccounts
    {
        public Guid Id { get; set; }
        public decimal GvtBalance { get; set; }

        public Portfolios Portfolio { get; set; }

        public ApplicationUser User { get; set; }
        public Guid UserId { get; set; }

        public ICollection<InvestmentRequests> InvestmentRequests { get; set; }
    }
}
