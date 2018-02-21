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
using GenesisVision.DataModel.Models;
using Microsoft.EntityFrameworkCore;

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

        public OperationResult RecalculateStatisticForPeriod(Guid periodId)
        {
            return InvokeOperations.InvokeOperation(() =>
            {
                var period = context.Periods
                                    .Include(x => x.InvestmentProgram)
                                    .ThenInclude(x => x.ManagerAccount)
                                    .ThenInclude(x => x.BrokerTradeServer)
                                    .First(x => x.Id == periodId);

                var query = context.ManagersAccountsTrades
                                   .Where(x => x.ManagerAccountId == period.InvestmentProgram.ManagerAccountId);

                switch (period.InvestmentProgram.ManagerAccount.BrokerTradeServer.Type)
                {
                    case BrokerTradeServerType.MetaTrader4:
                        query = query.Where(x => x.DateClose >= period.DateFrom && x.DateClose < period.DateTo.AddMinutes(1));
                        break;
                    case BrokerTradeServerType.MetaTrader5:
                        query = query.Where(x => x.Date >= period.DateFrom && x.Date < period.DateTo.AddMinutes(1));
                        break;
                    default:
                        throw new NotSupportedException();
                }

                var trades = query.ToList();

                decimal totalProfit;
                decimal avgProfit;
                decimal totalVolume;
                int tradesCount;

                // todo
            });
        }
    }
}
