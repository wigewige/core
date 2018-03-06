using GenesisVision.Common.Models;
using GenesisVision.Core.Services.Interfaces;
using GenesisVision.DataModel;
using GenesisVision.DataModel.Enums;
using GenesisVision.DataModel.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace GenesisVision.Core.Services
{
    public class StatisticService : IStatisticService
    {
        private readonly ApplicationDbContext context;

        public StatisticService(ApplicationDbContext context)
        {
            this.context = context;
        }

        public OperationResult GetInvestmentProgramStatistic(Guid invProgramId, DateTime? dateFrom = null, DateTime? dateTo = null)
        {
            return InvokeOperations.InvokeOperation(() =>
            {
                var statistic = context.ManagersAccountsStatistics
                                       .Where(x => x.ManagerAccount.InvestmentProgram.Id == invProgramId);

                if (dateFrom.HasValue)
                    statistic = statistic.Where(x => x.Date >= dateFrom.Value);
                if (dateTo.HasValue)
                    statistic = statistic.Where(x => x.Date <= dateTo);

                var result = statistic.ToList();

                return;
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
                        query = query.Where(x => x.DateClose >= period.DateFrom && x.DateClose < period.DateTo.AddSeconds(5));
                        break;
                    case BrokerTradeServerType.MetaTrader5:
                        query = query.Where(x => x.Date >= period.DateFrom && x.Date < period.DateTo.AddSeconds(5));
                        break;
                    default:
                        throw new NotSupportedException();
                }

                var trades = query.ToList();
                
                // one period - one statistic record
                var existPeriodStatistic = context.ManagersAccountsStatistics
                                                  .Where(x => x.ManagerAccountId == period.InvestmentProgram.ManagerAccountId &&
                                                              x.PeriodId == periodId)
                                                  .ToList();
                if (existPeriodStatistic.Any())
                    context.RemoveRange(existPeriodStatistic);
                //

                var totalProfit = trades.Where(x => x.Profit > 0).Sum(x => x.Profit);
                var totalLoss = trades.Where(x => x.Profit < 0).Sum(x => x.Profit);
                var totalVolume = trades.Sum(x => x.Volume);

                var statistic = context.ManagersAccountsStatistics
                                       .Where(x => x.ManagerAccountId == period.InvestmentProgram.ManagerAccountId)
                                       .OrderByDescending(x => x.Date)
                                       .ToList();

                context.Add(new ManagersAccountsStatistics
                            {
                                Id = Guid.NewGuid(),
                                ManagerAccountId = period.InvestmentProgram.ManagerAccountId,
                                Date = DateTime.UtcNow,
                                PeriodId = periodId,
                                Profit = totalProfit,
                                Loss = totalLoss,
                                Volume = totalVolume,
                                Fund = period.ManagerStartBalance,
                                TotalProfit = (totalProfit + totalLoss) + (statistic.FirstOrDefault()?.TotalProfit ?? 0m)
                            });

                period.InvestmentProgram.ManagerAccount.ProfitTotal = statistic.Sum(x => x.Profit + x.Loss);
                period.InvestmentProgram.ManagerAccount.ProfitAvg = statistic.Any() ? (statistic.Sum(x => x.Profit + x.Loss) / statistic.Count) : 0m;
                period.InvestmentProgram.ManagerAccount.VolumeTotal = statistic.Sum(x => x.Volume);
                period.InvestmentProgram.ManagerAccount.VolumeAvg = statistic.Any() ? (statistic.Sum(x => x.Volume) / statistic.Count) : 0m;

                context.SaveChanges();
            });
        }
    }
}
