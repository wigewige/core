using System;
using GenesisVision.Core.Data.Models;

namespace GenesisVision.Core.ViewModels.Investment
{
    public class Invest
    {
        public Guid InvestmentProgramId { get; set; }
        public Guid UserId { get; set; }
        public decimal Amount { get; set; }
        public InvestmentRequestType RequestType { get; set; }
    }
}
