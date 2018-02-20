using System;

namespace GenesisVision.Core.ViewModels.Trades
{
    public class MetaTrader4Order : BaseOrder
    {
        public DateTime DateOpen { get; set; }
        public DateTime DateClose { get; set; }
        public decimal PriceOpen { get; set; }
        public decimal PriceClose { get; set; }
    }
}
