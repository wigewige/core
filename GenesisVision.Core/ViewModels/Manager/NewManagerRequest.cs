using System;

namespace GenesisVision.Core.ViewModels.Manager
{
    public class NewManagerRequest
    {
        public Guid UserId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Currency { get; set; }
        public Guid BrokerTradeServerId { get; set; }
    }
}
