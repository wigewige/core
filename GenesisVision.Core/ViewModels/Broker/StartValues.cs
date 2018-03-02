using System;

namespace GenesisVision.Core.ViewModels.Broker
{
    public class StartValues
    {
        public Guid InvestmentProgramId { get; set; }
        public decimal Balance { get; set; }
        public decimal ManagerBalance { get; set; }
        public decimal ManagerShare { get; set; }
    }
}
