using System;
using System.Collections.Generic;

namespace GenesisVision.DataModel.Models
{
    public class BrokersAccounts
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime RegistrationDate { get; set; }
        public bool IsEnabled { get; set; }
        public string Logo { get; set; }

        public ApplicationUser User { get; set; }
        public Guid UserId { get; set; }

        public ICollection<BrokerTradeServers> BrokerTradeServers { get; set; }
    }
}
