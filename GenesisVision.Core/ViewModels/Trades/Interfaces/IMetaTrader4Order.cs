using System;

namespace GenesisVision.Core.ViewModels.Trades.Interfaces
{
    public interface IMetaTrader4Order : IBaseOrder
    {
        DateTime? DateOpen { get; set; }
        DateTime? DateClose { get; set; }
        decimal? PriceOpen { get; set; }
        decimal? PriceClose { get; set; }
    }
}
