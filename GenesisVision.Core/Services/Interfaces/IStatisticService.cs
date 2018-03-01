using GenesisVision.Common.Models;
using System;

namespace GenesisVision.Core.Services.Interfaces
{
    public interface IStatisticService
    {
        OperationResult GetInvestmentProgramStatistic(Guid invProgramId, DateTime? dateFrom = null, DateTime? dateTo = null);

        OperationResult RecalculateStatisticForPeriod(Guid periodId);
    }
}
