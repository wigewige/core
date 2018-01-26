using GenesisVision.Core.ViewModels.Common;
using System;

namespace GenesisVision.Core.ViewModels.Investment
{
    public class Invest : HiddenUserId
    {
        public Guid InvestmentProgramId { get; set; }
        public decimal Amount { get; set; }
    }
}
