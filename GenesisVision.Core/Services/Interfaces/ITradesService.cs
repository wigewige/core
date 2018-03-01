using GenesisVision.Common.Models;
using GenesisVision.Core.ViewModels.Trades;
using System.Collections.Generic;

namespace GenesisVision.Core.Services.Interfaces
{
    public interface ITradesService
    {
        OperationResult<(List<OrderModel>, int)> GetManagerTrades(TradesFilter filter);

        OperationResult<(List<OpenOrderModel>, int)> GetManagerOpenTrades(TradesFilter filter);

        OperationResult<List<OrderModel>> ConvertMetaTrader4OrdersFromCsv(string ipfsText);

        OperationResult SaveNewTrade(NewTradeEvent tradeEvent);

        OperationResult SaveNewOpenTrade(NewOpenTradesEvent openTradesEvent);
    }
}
