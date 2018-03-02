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
        [RegularExpression(@"^(?=.*\d)(?=.*[a-zA-Z])[a-zA-Z0-9]{8,}$", ErrorMessage = "Wrong password. Minimum 8 digits, symbols and numbers.")]
        public string TradePlatformPassword { get; set; }
        [Required]
        public decimal DepositAmount { get; set; }

        [Required]
        public string TokenName { get; set; }
        [Required]
        public string TokenSymbol { get; set; }
        
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public string Logo { get; set; }
        [Required]
        public string Title { get; set; }
        public string Description { get; set; }
        public decimal? FeeManagement { get; set; }
        public decimal? FeeSuccess { get; set; }
        public decimal? InvestMinAmount { get; set; }
        public decimal? InvestMaxAmount { get; set; }
        [Required]
        public int Period { get; set; }
    }
}
