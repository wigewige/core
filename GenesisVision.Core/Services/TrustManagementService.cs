using GenesisVision.Core.Helpers.Convertors;
using GenesisVision.Core.Models;
using GenesisVision.Core.Services.Interfaces;
using GenesisVision.Core.ViewModels.Broker;
using GenesisVision.Core.ViewModels.Investment;
using GenesisVision.DataModel;
using GenesisVision.DataModel.Enums;
using GenesisVision.DataModel.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GenesisVision.Core.Services
{
    public class TrustManagementService : ITrustManagementService
    {
        private readonly ApplicationDbContext context;

        public TrustManagementService(ApplicationDbContext context)
        {
            this.context = context;
        }
        
        public OperationResult CloseInvestmentProgram(Guid invProgramId)
        {
            return InvokeOperations.InvokeOperation(() =>
            {
                var investment = context.InvestmentPrograms
                                        .Include(x => x.Periods)
                                        .First(x => x.Id == invProgramId);

                investment.DateTo = DateTime.Now;

                var plannedPeriod = investment.Periods.FirstOrDefault(x => x.Status == PeriodStatus.Planned);
                if (plannedPeriod != null)
                {
                    plannedPeriod.Status = PeriodStatus.Closed;
                }

                context.SaveChanges();
            });
        }

        public OperationResult Invest(Invest model)
        {
            return InvokeOperations.InvokeOperation(() =>
            {
                var investor = context.InvestorAccounts
                                      .First(x => x.UserId == model.UserId);

                var lastPeriod = context.Periods
                                        .Where(x => x.InvestmentProgramId == model.InvestmentProgramId)
                                        .OrderByDescending(x => x.Number)
                                        .First();

                var invRequest = new InvestmentRequests
                                 {
                                     Id = Guid.NewGuid(),
                                     UserId = model.UserId,
                                     Amount = model.Amount,
                                     Date = DateTime.Now,
                                     InvestmentProgramtId = model.InvestmentProgramId,
                                     Status = InvestmentRequestStatus.New,
                                     Type = InvestmentRequestType.Invest,
                                     PeriodId = lastPeriod.Id,
                                     InvestorAccountId = investor.Id
                                 };

                context.Add(invRequest);
                context.SaveChanges();
            });
        }

        public OperationResult<(List<InvestmentProgram>, int)> GetInvestments(InvestmentsFilter filter)
        {
            return InvokeOperations.InvokeOperation(() =>
            {
                var query = context.InvestmentPrograms
                                   .Include(x => x.ManagerAccount)
                                   .Include(x => x.Token)
                                   .Include(x => x.Periods)
                                   .AsQueryable();

                if (filter.ManagerId.HasValue)
                    query = query.Where(x => x.ManagerAccountId == filter.ManagerId);
                if (filter.BrokerId.HasValue)
                    query = query.Where(x => x.ManagerAccount.BrokerTradeServer.BrokerId == filter.BrokerId);
                if (filter.BrokerTradeServerId.HasValue)
                    query = query.Where(x => x.ManagerAccount.BrokerTradeServerId == filter.BrokerTradeServerId);
                if (filter.InvestMaxAmountFrom.HasValue)
                    query = query.Where(x => x.InvestMinAmount >= filter.InvestMaxAmountFrom);
                if (filter.InvestMaxAmountTo.HasValue)
                    query = query.Where(x => x.InvestMaxAmount < filter.InvestMaxAmountTo);

                var count = query.Count();

                if (filter.Skip.HasValue)
                    query = query.Skip(filter.Skip.Value);
                if (filter.Take.HasValue)
                    query = query.Take(filter.Take.Value);

                var investments = query.Select(x => x.ToInvestmentProgram()).ToList();
                return (investments, count);
            });
        }

        public OperationResult<List<InvestmentProgram>> GetBrokerInvestmentsInitData(Guid brokerTradeServerId)
        {
            return InvokeOperations.InvokeOperation(() =>
            {
                var brokerInvestments = context.InvestmentPrograms
                                               .Include(x => x.ManagerAccount)
                                               .Include(x => x.Token)
                                               .Include(x => x.Periods)
                                               .Where(x =>
                                                   x.ManagerAccount.BrokerTradeServerId == brokerTradeServerId &&
                                                   x.IsEnabled &&
                                                   x.DateFrom < DateTime.Now &&
                                                   (x.DateTo == null || x.DateTo > DateTime.Now))
                                               .Select(x => x.ToInvestmentProgram())
                                               .ToList();
                return brokerInvestments;
            });
        }

        public OperationResult<ClosePeriodData> GetClosingPeriodData(Guid investmentProgramId)
        {
            return InvokeOperations.InvokeOperation(() =>
            {
                var investment = context.InvestmentPrograms
                                        .Include(x => x.Periods)
                                        .ThenInclude(x => x.InvestmentRequests)
                                        .First(x => x.Id == investmentProgramId);

                var data = new ClosePeriodData
                           {
                               NextPeriod = investment.Periods.FirstOrDefault(x => x.Status == PeriodStatus.Planned)?.ToPeriod(),
                               CurrentPeriod = investment.Periods.FirstOrDefault(x => x.Status == PeriodStatus.InProccess)?.ToPeriod()
                           };
                data.CanCloseCurrentPeriod = data.CurrentPeriod != null && data.CurrentPeriod.DateTo <= DateTime.Now;

                return data;
            });
        }

        public OperationResult ClosePeriod(Guid investmentProgramId)
        {
            return InvokeOperations.InvokeOperation(() =>
            {
                var investment = context.InvestmentPrograms
                                        .Include(x => x.Periods)
                                        .ThenInclude(x => x.InvestmentRequests)
                                        .First(x => x.Id == investmentProgramId);

                var currentPeriod = investment.Periods.First(x => x.Status == PeriodStatus.InProccess);
                currentPeriod.Status = PeriodStatus.Closed;

                // todo: proportional accrual of money
                
                var nextPeriod = investment.Periods.FirstOrDefault(x => x.Status == PeriodStatus.Planned);
                if (nextPeriod != null)
                {
                    nextPeriod.Status = PeriodStatus.InProccess;

                    var investments = nextPeriod.InvestmentRequests
                                                .Where(x => x.Type == InvestmentRequestType.Invest &&
                                                            x.Status == InvestmentRequestStatus.New)
                                                .ToList();
                    nextPeriod.StartBalance = investments.Sum(x => x.Amount);
                    investments.ForEach(x => x.Status = InvestmentRequestStatus.Executed);
                }

                var cancelledPeriod = investment.Periods.FirstOrDefault(x =>
                    x.Status == PeriodStatus.Closed &&
                    x.Number == currentPeriod.Number + 1);
                if (cancelledPeriod != null)
                {
                    var pendingInvests = cancelledPeriod.InvestmentRequests.Where(x =>
                        x.Type == InvestmentRequestType.Invest &&
                        x.Status == InvestmentRequestStatus.New);

                    if (pendingInvests.Any())
                    {
                        // todo: return money
                    }
                }

                if (!investment.DateTo.HasValue || DateTime.Now < investment.DateTo.Value)
                {
                    var newPeriod = new Periods
                                    {
                                        Id = Guid.NewGuid(),
                                        DateFrom = DateTime.Now,
                                        DateTo = DateTime.Now.AddDays(investment.Period),
                                        InvestmentProgramId = investmentProgramId,
                                        Number = investment.Periods.Max(x => x.Number) + 1,
                                        Status = PeriodStatus.Planned
                                    };
                    context.Add(newPeriod);
                }

                if (investment.DateTo.HasValue && investment.DateTo < DateTime.Now)
                {
                    investment.IsEnabled = false;
                }

                context.SaveChanges();
            });
        }

        public OperationResult SetPeriodStartBalance(Guid periodId, decimal balance)
        {
            return InvokeOperations.InvokeOperation(() =>
            {
                var period = context.Periods.First(x => x.Id == periodId);
                period.StartBalance = balance;
                context.SaveChanges();
            });
        }

        public OperationResult<(List<BrokerTradeServer>, int)> GetBrokerTradeServers(BrokersFilter filter)
        {
            return InvokeOperations.InvokeOperation(() =>
            {
                var query = context.BrokerTradeServers
                                   .Include(x => x.Broker)
                                   .Where(x => x.IsEnabled && x.Broker.IsEnabled);

                if (!string.IsNullOrEmpty(filter.TradeServerName))
                    query = query.Where(x => x.Name.ToLower().Contains(filter.TradeServerName.ToLower()));

                if (filter.TradeServerType.HasValue)
                    query = query.Where(x => x.Type == filter.TradeServerType);

                if (!string.IsNullOrEmpty(filter.BrokerName))
                    query = query.Where(x => x.Broker.Name.ToLower().Contains(filter.BrokerName.ToLower()));

                var count = query.Count();

                if (filter.Skip.HasValue)
                    query = query.Skip(filter.Skip.Value);
                if (filter.Take.HasValue)
                    query = query.Take(filter.Take.Value);

                var brokerTradeServers = query.Select(x => x.ToBrokerTradeServers()).ToList();
                return (brokerTradeServers, count);
            });
        }
    }
}
