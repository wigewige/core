using System;
using GenesisVision.DataModel.Enums;

namespace GenesisVision.Core.ViewModels.Trades.Interfaces
{
    public interface IBaseOrder
    {
        Guid Id { get; set; }
        long Ticket { get; set; }
        string Symbol { get; set; }
        decimal Volume { get; set; }
        decimal Profit { get; set; }
        TradeDirectionType Direction { get; set; }
    }
}
