using System;

namespace GenesisVision.Core.ViewModels.Trades
{
    public abstract class BaseTrade
    {
        public string Symbol { get; set; }
        public DateTime DateOpen { get; set; }
        public DateTime DateClose { get; set; }
        public decimal PriceOpen { get; set; }
        public decimal PriceClose { get; set; }
        public decimal Profit { get; set; }
        public decimal Volume { get; set; }
    }
}
