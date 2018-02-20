using GenesisVision.Core.Models;
using GenesisVision.Core.ViewModels.Trades;
using System;
using System.Collections.Generic;

namespace GenesisVision.Core.Services.Interfaces
{
    public interface ITradesService
    {
        OperationResult<List<MetaTrader4Order>> GetMetaTrader4Orders(Guid accountId, DateTime? dateFrom = null, DateTime? dateTo = null);

        OperationResult<List<MetaTrader5Order>> GetMetaTrader5Orders(Guid accountId, DateTime? dateFrom = null, DateTime? dateTo = null);

        OperationResult<List<MetaTrader4Order>> ConvertMetaTrader4OrdersFromCsv(string ipfsText);
    }
}
