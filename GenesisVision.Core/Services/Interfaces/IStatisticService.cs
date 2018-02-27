using GenesisVision.Core.Models;
using GenesisVision.Core.ViewModels.Investment;
using System;
using System.Collections.Generic;

namespace GenesisVision.Core.Services.Interfaces
{
    public interface IStatisticService
    {
        OperationResult GetInvestmentProgramStatistic(Guid invProgramId, DateTime? dateFrom = null, DateTime? dateTo = null);

        OperationResult RecalculateStatisticForPeriod(Guid periodId);
    }
}
