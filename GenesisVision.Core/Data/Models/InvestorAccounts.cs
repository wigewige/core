using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GenesisVision.Core.Data.Models
{
    public class InvestorAccounts
    {
        public Guid Id { get; set; }

        public AspNetUsers User { get; set; }
        public Guid UserId { get; set; }
        public Portfolios Portfolio { get; set; }
        public decimal GvtBalance { get; set; }
    }
}
