using System;

namespace GenesisVision.Core.ViewModels.Investment
{
    public class Chart
    {
        public DateTime Date { get; set; }
        public decimal Fund { get; set; }
        public decimal Profit { get; set; }
        public decimal Loss { get; set; }
        public decimal TotalProfit { get; set; }
    }
}
