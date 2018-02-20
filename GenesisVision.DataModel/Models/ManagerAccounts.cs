using System;
using System.Collections.Generic;

namespace GenesisVision.DataModel.Models
{
    public class ManagerAccounts
    {
        public Guid Id { get; set; }
        public string Login { get; set; }
        public string Currency { get; set; }
        public DateTime RegistrationDate { get; set; }
        public string IpfsHash { get; set; }
        public bool IsConfirmed { get; set; }

        public ApplicationUser User { get; set; }
        public Guid UserId { get; set; }

        public BrokerTradeServers BrokerTradeServer { get; set; }
        public Guid BrokerTradeServerId { get; set; }

        public InvestmentPrograms InvestmentProgram { get; set; }

        public ICollection<ManagersAccountsStatistics> ManagersAccountsStatistics { get; set; }
        public ICollection<ManagersAccountsTrades> ManagersAccountsTrades { get; set; }
    }
}
