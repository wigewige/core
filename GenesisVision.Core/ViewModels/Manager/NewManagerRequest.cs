using System;

namespace GenesisVision.Core.ViewModels.Manager
{
    public class NewManagerRequest : ManagerRequest
    {
        public Guid? UserId { get; set; }
        public Guid BrokerTradeServerId { get; set; }
    }
}
