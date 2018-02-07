using System;
using System.ComponentModel.DataAnnotations;
using GenesisVision.Core.ViewModels.Common;

namespace GenesisVision.Core.ViewModels.Manager
{
    public class NewInvestmentRequest : HiddenUserId
    {
        [Required]
        public Guid BrokerTradeServerId { get; set; }

        [Required]
        public string TradePlatformPassword { get; set; }
        [Required]
        public decimal DepositAmount { get; set; }

        [Required]
        public string TokenName { get; set; }
        [Required]
        public string TokenSymbol { get; set; }

        [Required]
        public DateTime? DateFrom { get; set; }
        [Required]
        public DateTime? DateTo { get; set; }
        public string Logo { get; set; }
        [Required]
        public string Title { get; set; }
        public string Description { get; set; }
        [Required]
        public decimal FeeEntrance { get; set; }
        [Required]
        public decimal FeeManagement { get; set; }
        [Required]
        public decimal FeeSuccess { get; set; }
        [Required]
        public decimal InvestMinAmount { get; set; }
        [Required]
        public decimal? InvestMaxAmount { get; set; }
        [Required]
        public int Period { get; set; }
    }
}
