using GenesisVision.Core.ViewModels.Broker;
using System;

namespace GenesisVision.Core.ViewModels.Investment
{
    public class InvestmentProgram
    {
        public Investment Investment { get; set; }

        public ManagerAccount Account { get; set; }

        public ManagerToken Token { get; set; }
    }

    public class Investment : InvestmentShort
    {
        public DateTime DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public Period LastPeriod { get; set; }
        public int Period { get; set; }
        public decimal FeeSuccess { get; set; }
        public decimal FeeManagement { get; set; }
        public decimal FeeEntrance { get; set; }
        public decimal InvestMinAmount { get; set; }
        public decimal? InvestMaxAmount { get; set; }
    }

    public class InvestmentShort
    {
        public Guid Id { get; set; }
        public Guid ManagerAccountId { get; set; }
        public Guid ManagerTokensId { get; set; }
        public string Logo { get; set; }
        public string Description { get; set; }
        public bool IsEnabled { get; set; }
        public decimal Rating { get; set; }
        public int OrdersCount { get; set; }
        public decimal TotalProfit { get; set; }
    }

    public class ManagerAccount
    {
        public Guid Id { get; set; }
        public string Login { get; set; }
        public string Currency { get; set; }
        public string IpfsHash { get; set; }
        public bool IsConfirmed { get; set; }
        public DateTime RegistrationDate { get; set; }
        public BrokerTradeServer BrokerTradeServer { get; set; }
    }

    public class ManagerToken
    {
        public Guid Id { get; set; }
        public string TokenSymbol { get; set; }
        public string TokenName { get; set; }
        public string TokenAddress { get; set; }
    }
}
