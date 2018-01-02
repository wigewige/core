using System;
using System.Collections.Generic;

namespace GenesisVision.DataModel.Models
{
    public class Brokers
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime RegistrationDate { get; set; }
        public bool IsEnabled { get; set; }
        public ICollection<BrokerTradeServers> BrokerTradeServers { get; set; }
    }
}
