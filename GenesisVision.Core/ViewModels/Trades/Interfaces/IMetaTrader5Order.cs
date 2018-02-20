using System;
using GenesisVision.DataModel.Enums;

namespace GenesisVision.Core.ViewModels.Trades.Interfaces
{
    public interface IMetaTrader5Order : IBaseOrder
    {
        DateTime? Date { get; set; }
        decimal? Price { get; set; }
        TradeEntryType? Entry { get; set; }
    }
}
