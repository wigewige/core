using System;
using System.Collections.Generic;
using System.Linq;
using GenesisVision.DataModel;
using GenesisVision.DataModel.Models;
using GenesisVision.Core.Helpers;
using GenesisVision.Core.Models;
using GenesisVision.Core.Services.Interfaces;
using GenesisVision.Core.ViewModels.Broker;
using GenesisVision.Core.ViewModels.Investment;
using GenesisVision.Core.ViewModels.Manager;
using Microsoft.EntityFrameworkCore;

namespace GenesisVision.Core.Services
{
    public class TrustManagementService : ITrustManagementService
    {
        private readonly ApplicationDbContext context;

        public TrustManagementService(ApplicationDbContext context)
        {
            this.context = context;
        }

        public OperationResult<Guid> CreateInvestmentProgram(CreateInvestment investment)
        {
            return InvokeOperations.InvokeOperation(() =>
            {
                var token = new ManagerTokens
                            {
                                Id = Guid.NewGuid(),
                                TokenAddress = string.Empty,
                                TokenSymbol = string.Empty
                            };
                var inv = new InvestmentPrograms
                          {
                              Id = Guid.NewGuid(),
                              DateFrom = investment.DateFrom ?? DateTime.Now,
                              DateTo = investment.DateTo,
                              Description = investment.Description,
                              FeeEntrance = investment.FeeEntrance,
                              FeeManagement = investment.FeeManagement,
                              FeeSuccess = investment.FeeSuccess,
                              InvestMaxAmount = investment.InvestMaxAmount,
                              InvestMinAmount = investment.InvestMinAmount,
                              IsEnabled = true,
                              ManagersAccountId = investment.ManagersAccountId,
                              Period = investment.Period,
                              ManagerTokensId = token.Id
                          };
                var firstPeriod = new Periods
                                  {
                                      Id = Guid.NewGuid(),
                                      DateFrom = inv.DateFrom,
                                      DateTo = inv.DateFrom.AddDays(inv.Period),
                                      Status = PeriodStatus.InProccess,
                                      InvestmentProgramId = inv.Id,
                                      Number = 1
                                  };
                context.Add(inv);
                context.Add(firstPeriod);
                context.Add(token);
                context.SaveChanges();

                return inv.Id;
            });
        }

        public OperationResult Invest(Invest model)
        {
            return InvokeOperations.InvokeOperation(() =>
            {
                var investor = context.AspNetUsers
                                      .Include(x => x.InvestorAccount)
                                      .First(x => x.Id == model.UserId);

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
                                     InvestorAccountId = investor.InvestorAccount.Id
                                 };

                context.Add(invRequest);
                context.SaveChanges();
            });
        }

        public OperationResult<(List<Investment>, int)> GetInvestments(InvestmentsFilter filter)
        {
            return InvokeOperations.InvokeOperation(() =>
            {
                var query = context.InvestmentPrograms.AsQueryable();

                if (filter.ManagerId.HasValue)
                    query = query.Where(x => x.ManagersAccountId == filter.ManagerId);
                if (filter.BrokerId.HasValue)
                    query = query.Where(x => x.ManagersAccount.BrokerTradeServer.BrokerId == filter.BrokerId);
                if (filter.BrokerTradeServerId.HasValue)
                    query = query.Where(x => x.ManagersAccount.BrokerTradeServerId == filter.BrokerTradeServerId);
                if (filter.InvestMaxAmountFrom.HasValue)
                    query = query.Where(x => x.InvestMinAmount >= filter.InvestMaxAmountFrom);
                if (filter.InvestMaxAmountTo.HasValue)
                    query = query.Where(x => x.InvestMaxAmount < filter.InvestMaxAmountTo);

                var count = query.Count();

                if (filter.Skip.HasValue)
                    query = query.Skip(filter.Skip.Value);
                if (filter.Take.HasValue)
                    query = query.Take(filter.Take.Value);

                var investments = query.Select(x => x.ToInvestment()).ToList();
                return (investments, count);
            });
        }

        public OperationResult<List<Investment>> GetBrokerInvestmentsInitData(Guid brokerTradeServerId)
        {
            return InvokeOperations.InvokeOperation(() =>
            {
                var brokerInvestments = context.InvestmentPrograms
                                               .Include(x => x.Periods)
                                               .Where(x =>
                                                   x.ManagersAccount.BrokerTradeServerId == brokerTradeServerId &&
                                                   x.IsEnabled &&
                                                   x.DateFrom < DateTime.Now &&
                                                   (x.DateTo == null || x.DateTo > DateTime.Now))
                                               .Select(x => x.ToInvestment())
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
    }
}
