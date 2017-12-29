using System;

namespace GenesisVision.Core.ViewModels.Manager
{
    public class ManagerAccount
    {
        public Guid Id { get; set; }
        public Broker.Broker Broker { get; set; }
        public Broker.BrokerTradeServer BrokerTradeServer { get; set; }
        public string Name { get; set; }
        public string Avatar { get; set; }
        public string Login { get; set; }
    }
}
