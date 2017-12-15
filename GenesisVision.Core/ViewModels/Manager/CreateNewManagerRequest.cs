using System;

namespace GenesisVision.Core.ViewModels.Manager
{
    public class CreateNewManagerRequest
    {
        public string Name { get; set; }
        public Guid BrokerId { get; set; }
        public Guid BrokerTradeServerId { get; set; }
        public decimal ManagementFee { get; set; }
        public decimal SuccessFee { get; set; }
    }
}
