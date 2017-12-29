using System;
using GenesisVision.Core.ViewModels;
using GenesisVision.Core.ViewModels.Broker;

namespace GenesisVision.Core.ViewModels.Manager
{
    public class ManagerAccount
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Avatar { get; set; }
        public string Login { get; set; }
        public string Description { get; set; }
        public string Currency { get; set; }
        public string IpfsHash { get; set; }
        public bool IsEnabled { get; set; }
        public Broker.Broker Broker { get; set; }
        public BrokerTradeServer BrokerTradeServer { get; set; }
    }
}
