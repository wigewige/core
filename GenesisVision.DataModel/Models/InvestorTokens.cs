using System;

namespace GenesisVision.DataModel.Models
{
    public class InvestorTokens
    {
        public Guid Id { get; set; }

        public Guid InvestorAccountId { get; set; }
        public InvestorAccounts InvestorAccount { get; set; }

        public decimal Amount { get; set; }

        public ManagerTokens ManagerToken { get; set; }
        public Guid ManagerTokenId { get; set; }
    }
}
    