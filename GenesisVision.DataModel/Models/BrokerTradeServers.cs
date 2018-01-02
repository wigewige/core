using System;
using System.Collections.Generic;
using GenesisVision.DataModel.Enums;

namespace GenesisVision.DataModel.Models
{
    public class BrokerTradeServers
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Host { get; set; }
        public BrokerTradeServerType Type { get; set; }
        public DateTime RegistrationDate { get; set; }
        public bool IsEnabled { get; set; }

        public Guid BrokerId { get; set; }
        public Brokers Broker { get; set; }

        public ICollection<ManagerAccounts> ManagerAccounts { get; set; }

        public ICollection<ManagerAccountRequests> ManagerAccountRequests { get; set; }
    }
}
