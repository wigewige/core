using System;
using System.Collections.Generic;

namespace GenesisVision.Core.Data.Models
{
    public class InvestorAccounts
    {
        public Guid Id { get; set; }
        public decimal GvtBalance { get; set; }

        public Portfolios Portfolio { get; set; }

        public AspNetUsers User { get; set; }
        public Guid UserId { get; set; }

        public ICollection<InvestmentRequests> InvestmentRequestses { get; set; }
    }
}
