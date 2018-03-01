using System;
using GenesisVision.Core.ViewModels.Investment;

namespace GenesisVision.Core.ViewModels.Broker
{
    public class BrokerInvestmentProgram
    {
        public Guid Id { get; set; }
        public Guid ManagerAccountId { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public string Description { get; set; }
        public bool IsEnabled { get; set; }
        public int Period { get; set; }
        public decimal FeeSuccess { get; set; }
        public decimal FeeManagement { get; set; }
        public decimal FeeEntrance { get; set; }
        public decimal InvestMinAmount { get; set; }
        public decimal? InvestMaxAmount { get; set; }
        public Period LastPeriod { get; set; }
        public string Login { get; set; }
        public string IpfsHash { get; set; }
        public string TradeIpfsHash { get; set; }
    }
}
