using System;
using GenesisVision.Core.ViewModels.Common;

namespace GenesisVision.Core.ViewModels.Manager
{
    public class NewInvestmentRequest : HiddenUserId
    {
        public Guid BrokerTradeServerId { get; set; }

        public string TradePlatformPassword { get; set; }
        public decimal DepositAmount { get; set; }

        public string TokenName { get; set; }
        public string TokenSymbol { get; set; }
        
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public string Logo { get; set; }
        public string Description { get; set; }
        public decimal FeeEntrance { get; set; }
        public decimal FeeManagement { get; set; }
        public decimal FeeSuccess { get; set; }
        public decimal InvestMinAmount { get; set; }
        public decimal? InvestMaxAmount { get; set; }
        public int Period { get; set; }
    }
}
