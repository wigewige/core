using GenesisVision.Core.Models;
using GenesisVision.Core.ViewModels.Trades;
using System;
using System.Collections.Generic;
using GenesisVision.Core.ViewModels.Trades.Interfaces;

namespace GenesisVision.Core.Services.Interfaces
{
    public interface ITradesService
    {
        OperationResult<List<OrderModel>> GetOrders(Guid accountId, DateTime? dateFrom = null, DateTime? dateTo = null);

        OperationResult<List<OrderModel>> ConvertMetaTrader4OrdersFromCsv(string ipfsText);

        OperationResult SaveNewTrade(NewTradeEvent tradeEvent);
    }
}
