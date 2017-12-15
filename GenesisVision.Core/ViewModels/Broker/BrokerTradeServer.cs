using System;
using GenesisVision.Core.Data.Models;

namespace GenesisVision.Core.ViewModels.Broker
{
    public class BrokerTradeServer
    {
        public Guid Id { get; set; }
        public Guid BrokerId { get; set; }
        public string Name { get; set; }
        public string Host { get; set; }
        public BrokerTradeServerType Type { get; set; }
    }
}
