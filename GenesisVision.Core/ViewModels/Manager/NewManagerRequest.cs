using System;

namespace GenesisVision.Core.ViewModels.Manager
{
    public class NewManagerRequest : ManagerRequest
    {
        public Guid BrokerTradeServerId { get; set; }
    }
}
