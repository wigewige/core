using System;
using GenesisVision.Core.ViewModels.Common;

namespace GenesisVision.Core.ViewModels.Trades
{
    public class TradesFilter : PagingFilter
    {
        public Guid? ManagerId { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public string Symbol { get; set; }
    }
}
