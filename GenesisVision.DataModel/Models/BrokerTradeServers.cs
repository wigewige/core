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
        public int HoursOffset { get; set; }

        public Guid BrokerId { get; set; }
        public BrokersAccounts Broker { get; set; }

        public ICollection<ManagerAccounts> ManagerAccounts { get; set; }

        public ICollection<ManagerRequests> ManagerAccountRequests { get; set; }
    }
}
