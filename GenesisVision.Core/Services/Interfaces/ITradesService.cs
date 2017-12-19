using System.Collections.Generic;
using GenesisVision.Core.Models;
using GenesisVision.Core.ViewModels.Trades;

namespace GenesisVision.Core.Services.Interfaces
{
    public interface ITradesService
    {
        OperationResult<List<MetaTraderOrder>> ConvertMetaTraderOrdersFromCsv(string ipfsText);
    }
}
