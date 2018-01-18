using GenesisVision.DataModel.Enums;
using System;

namespace GenesisVision.DataModel.Models
{
    public class ManagerRequests
    {
        public Guid Id { get; set; }
        public DateTime Date { get; set; }
        public ManagerRequestType Type { get; set; }
        public ManagerRequestStatus Status { get; set; }
        
        public decimal DepositAmount { get; set; }
        public string TradePlatformCurrency { get; set; }
        public string TradePlatformPassword { get; set; }

        public string TokenName { get; set; }
        public string TokenSymbol { get; set; }

        public string Logo { get; set; }
        public string Description { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public int Period { get; set; }
        public decimal FeeSuccess { get; set; }
        public decimal FeeManagement { get; set; }
        public decimal FeeEntrance { get; set; }
        public decimal InvestMinAmount { get; set; }
        public decimal? InvestMaxAmount { get; set; }

        public ApplicationUser User { get; set; }
        public Guid UserId { get; set; }

        public BrokerTradeServers BrokerTradeServers { get; set; }
        public Guid BrokerTradeServerId { get; set; }
    }
}
