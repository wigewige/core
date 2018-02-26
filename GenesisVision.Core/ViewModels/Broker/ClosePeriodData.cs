using GenesisVision.Core.ViewModels.Investment;
using System.Collections.Generic;

namespace GenesisVision.Core.ViewModels.Broker
{
    public class ClosePeriodData
    {
        public Period CurrentPeriod { get; set; }

        public List<InvestorAmount> TokenHolders { get; set; }

        public bool CanCloseCurrentPeriod { get; set; }
    }
}
