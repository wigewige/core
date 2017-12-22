using System;
using System.Collections.Generic;

namespace GenesisVision.Core.Data.Models
{
    public enum BrokerTradeServerType
    {
        Undefined = 0,
        MetaTrader4 = 1,
        MetaTrader5 = 2,
        NinjaTrader = 3,
        cTrader = 4,
        Rumus = 5,
        Metastock = 6,
    }

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
