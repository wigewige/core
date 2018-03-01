using System;
using System.Collections.Generic;
using GenesisVision.DataModel.Enums;

namespace GenesisVision.DataModel.Models
{
    public class ManagerAccounts
    {
        public Guid Id { get; set; }
        public string Login { get; set; }
        public Currency Currency { get; set; }
        public DateTime RegistrationDate { get; set; }
        public string IpfsHash { get; set; }
        public string TradeIpfsHash { get; set; }
        public bool IsConfirmed { get; set; }

        public ApplicationUser User { get; set; }
        public Guid UserId { get; set; }

        public BrokerTradeServers BrokerTradeServer { get; set; }
        public Guid BrokerTradeServerId { get; set; }

        public InvestmentPrograms InvestmentProgram { get; set; }

        public ICollection<ManagersAccountsStatistics> ManagersAccountsStatistics { get; set; }
        public ICollection<ManagersAccountsTrades> ManagersAccountsTrades { get; set; }
        public ICollection<ManagersAccountsOpenTrades> ManagersAccountsOpenTrades { get; set; }
    }
}
