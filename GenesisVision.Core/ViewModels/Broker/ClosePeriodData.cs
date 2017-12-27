using GenesisVision.Core.ViewModels.Investment;

namespace GenesisVision.Core.ViewModels.Broker
{
    public class ClosePeriodData
    {
        public Period CurrentPeriod { get; set; }
        public Period NextPeriod { get; set; }
        public bool CanCloseCurrentPeriod { get; set; }
    }
}
