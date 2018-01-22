using GenesisVision.Core.Models;
using GenesisVision.Core.ViewModels.Investment;
using System;
using System.Collections.Generic;

namespace GenesisVision.Core.Services.Interfaces
{
    public interface IStatisticService
    {
        OperationResult<List<InvestmentProgramStatistic>> GetInvestmentProgramStatistic(Guid invProgramId, DateTime? dateFrom = null, DateTime? dateTo = null);
    }
}
