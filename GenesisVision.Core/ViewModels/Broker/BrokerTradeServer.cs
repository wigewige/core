using System;
using GenesisVision.DataModel.Enums;
using GenesisVision.DataModel.Models;

namespace GenesisVision.Core.ViewModels.Broker
{
    public class BrokerTradeServer
    {
        public Guid Id { get; set; }
        public Guid BrokerId { get; set; }
        public string Name { get; set; }
        public string Host { get; set; }
        public BrokerTradeServerType Type { get; set; }
        public Broker Broker { get; set; }
        public DateTime RegistrationDate { get; set; }
    }
}
