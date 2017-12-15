using System;
using System.Collections.Generic;

namespace GenesisVision.Core.Data.Models
{
    public class ManagerAccounts
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Avatar { get; set; }
        public string Login { get; set; }
        public string Currency { get; set; }
        public bool IsEnabled { get; set; }
        public decimal Rating { get; set; }
        public DateTime RegistrationDate { get; set; }
        
        public AspNetUsers User { get; set; }
        public Guid UserId { get; set; }

        public BrokerTradeServers BrokerTradeServer { get; set; }
        public Guid BrokerTradeServerId { get; set; }

        public ICollection<InvestmentPrograms> InvestmentPrograms { get; set; }
    }
}
