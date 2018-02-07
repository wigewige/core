using GenesisVision.Core.ViewModels.Common;
using System;
using System.ComponentModel.DataAnnotations;

namespace GenesisVision.Core.ViewModels.Investment
{
    public class Invest : HiddenUserId
    {
        [Required]
        public Guid InvestmentProgramId { get; set; }
        [Required]
        public decimal Amount { get; set; }
    }
}
