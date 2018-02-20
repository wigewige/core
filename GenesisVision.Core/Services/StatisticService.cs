using System;
using System.Collections.Generic;
using System.Linq;
using GenesisVision.Core.Helpers.Convertors;
using GenesisVision.Core.Models;
using GenesisVision.Core.Services.Interfaces;
using GenesisVision.Core.ViewModels.Investment;
using GenesisVision.Core.ViewModels.Trades;
using GenesisVision.DataModel;
using GenesisVision.DataModel.Enums;

namespace GenesisVision.Core.Services
{
    public class StatisticService : IStatisticService
    {
        private readonly ApplicationDbContext context;

        public StatisticService(ApplicationDbContext context)
        {
            this.context = context;
        }

        public OperationResult<List<InvestmentProgramStatistic>> GetInvestmentProgramStatistic(Guid invProgramId, DateTime? dateFrom = null, DateTime? dateTo = null)
        {
            return InvokeOperations.InvokeOperation(() =>
            {
                var statistic = context.ManagersAccountsStatistics
                                       .Where(x => x.InvestmentProgramId == invProgramId);

                if (dateFrom.HasValue)
                    statistic = statistic.Where(x => x.Date >= dateFrom.Value);
                if (dateTo.HasValue)
                    statistic = statistic.Where(x => x.Date <= dateTo);

                var result = statistic
                    .Select(x => x.ToInvestmentProgramStatistic())
                    .ToList();

                return result;
            });
        }
    }
}
