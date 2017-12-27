using System;

namespace GenesisVision.Core.ViewModels.Investment
{
    public class Invest
    {
        public Guid InvestmentProgramId { get; set; }
        public Guid UserId { get; set; }
        public decimal Amount { get; set; }
    }
}
