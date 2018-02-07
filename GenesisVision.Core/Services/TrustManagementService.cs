using GenesisVision.Core.Helpers.Convertors;
using GenesisVision.Core.Models;
using GenesisVision.Core.Services.Interfaces;
using GenesisVision.Core.ViewModels.Broker;
using GenesisVision.Core.ViewModels.Investment;
using GenesisVision.DataModel;
using GenesisVision.DataModel.Enums;
using GenesisVision.DataModel.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GenesisVision.Core.Services
{
    public class TrustManagementService : ITrustManagementService
    {
        private readonly ApplicationDbContext context;
        private readonly IIpfsService ipfsService;
        private readonly ISmartContractService smartContractService;

        public TrustManagementService(ApplicationDbContext context, IIpfsService ipfsService, ISmartContractService smartContractService)
        {
            this.context = context;
            this.ipfsService = ipfsService;
            this.smartContractService = smartContractService;
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

                    var pendingInvests = plannedPeriod.InvestmentRequests
                                                      .Where(x => x.Type == InvestmentRequestType.Invest &&
                                                                  x.Status == InvestmentRequestStatus.New)
                                                      .ToList();
                    if (pendingInvests.Any())
                    {
                        foreach (var request in pendingInvests)
                        {
                            var wallet = context.Wallets.First(x => x.UserId == request.UserId);
                            wallet.Amount += request.Amount;

                            var tx = new WalletTransactions
                                     {
                                         Id = Guid.NewGuid(),
                                         Type = WalletTransactionsType.WithdrawFromProgram,
                                         UserId = request.UserId,
                                         Amount = request.Amount,
                                         Date = DateTime.Now
                                     };
                            context.Add(tx);
                        }
                        pendingInvests.ForEach(x => x.Status = InvestmentRequestStatus.Executed);
                    }
                }

                context.SaveChanges();
            });
        }


        public OperationResult<Guid> CreateInvestmentProgram(NewManager request)
        {
            return InvokeOperations.InvokeOperation(() =>
            {
                var managerRequest = context.ManagerRequests.First(x => x.Id == request.RequestId && x.Status == ManagerRequestStatus.Created);
                managerRequest.TradePlatformPassword = null;
                managerRequest.Status = ManagerRequestStatus.Processed;

                var manager = new ManagerAccounts
                              {
                                  Id = Guid.NewGuid(),
                                  BrokerTradeServerId = managerRequest.BrokerTradeServerId,
                                  UserId = managerRequest.UserId,
                                  RegistrationDate = DateTime.Now,
                                  Login = request.Login,
                                  Currency = managerRequest.TradePlatformCurrency,
                                  IsConfirmed = false,
                                  IpfsHash = string.Empty
                              };
                var token = new ManagerTokens
                            {
                                Id = Guid.NewGuid(),
                                TokenAddress = string.Empty,
                                TokenName = managerRequest.TokenName,
                                TokenSymbol = managerRequest.TokenSymbol
                            };
                var inv = new InvestmentPrograms
                          {
                              Id = Guid.NewGuid(),
                              DateFrom = managerRequest.DateFrom,
                              DateTo = managerRequest.DateTo,
                              Description = managerRequest.Description,
                              Title = managerRequest.Title,
                              FeeEntrance = managerRequest.FeeEntrance,
                              FeeManagement = managerRequest.FeeManagement,
                              FeeSuccess = managerRequest.FeeSuccess,
                              InvestMaxAmount = managerRequest.InvestMaxAmount,
                              InvestMinAmount = managerRequest.InvestMinAmount,
                              IsEnabled = true,
                              ManagerAccountId = manager.Id,
                              Period = managerRequest.Period,
                              ManagerTokensId = token.Id,
                              Logo = managerRequest.Logo,
                              Rating = 0,
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
                if (!inv.DateTo.HasValue || inv.DateTo > inv.DateFrom.AddDays(inv.Period))
                {
                    var plannedPeriod = new Periods
                                        {
                                            Id = Guid.NewGuid(),
                                            DateFrom = firstPeriod.DateTo,
                                            DateTo = firstPeriod.DateTo.AddDays(inv.Period),
                                            Status = PeriodStatus.Planned,
                                            InvestmentProgramId = inv.Id,
                                            Number = 2
                                        };
                    context.Add(plannedPeriod);
                }

                context.Add(manager);
                context.Add(token);
                context.Add(inv);
                context.Add(firstPeriod);
                context.SaveChanges();

                var blockchainUpdate = smartContractService.RegisterManager(token.TokenName, token.TokenSymbol,
                    manager.Id.ToString(), manager.Login,
                    manager.BrokerTradeServerId.ToString(), inv.FeeManagement, inv.FeeSuccess);

                if (blockchainUpdate.IsSuccess)
                {
                    manager.IsConfirmed = true;
                    context.SaveChanges();

                    var ipfsUpdate = UpdateInvestmentInIpfs(inv.Id);
                    if (ipfsUpdate.IsSuccess)
                    {
                        manager.IpfsHash = ipfsUpdate.Data;
                        context.SaveChanges();
                    }
                }

                return manager.Id;
            });
        }

        public OperationResult Invest(Invest model)
        {
            return InvokeOperations.InvokeOperation(() =>
            {
                var investor = context.InvestorAccounts
                                      .Include(x => x.User)
                                      .ThenInclude(x => x.Wallet)
                                      .First(x => x.UserId == model.UserId);

                var lastPeriod = context.Periods
                                        .Where(x => x.InvestmentProgramId == model.InvestmentProgramId)
                                        .OrderByDescending(x => x.Number)
                                        .First();

                var tx = new WalletTransactions
                         {
                             Id = Guid.NewGuid(),
                             Type = WalletTransactionsType.InvestToProgram,
                             UserId = model.UserId,
                             Amount = model.Amount,
                             Date = DateTime.Now
                         };

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
                                     WalletTransactionId = tx.Id
                                 };

                investor.User.Wallet.Amount -= model.Amount;
                
                context.Add(invRequest);
                context.Add(tx);
                context.SaveChanges();
            });
        }

        public OperationResult RequestForWithdraw(Invest model)
        {
            return InvokeOperations.InvokeOperation(() =>
            {
                var investor = context.InvestorAccounts
                                      .First(x => x.UserId == model.UserId);

                var period = context.Periods
                                    .FirstOrDefault(x => x.InvestmentProgramId == model.InvestmentProgramId &&
                                                         x.Status == PeriodStatus.InProccess) ??
                             context.Periods
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
                                     Type = InvestmentRequestType.Withdrawal,
                                     PeriodId = period.Id
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
                                   .ThenInclude(x => x.User)
                                   .ThenInclude(x => x.Profile)
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

                if (filter.Sorting.HasValue)
                {
                    switch (filter.Sorting.Value)
                    {
                        case Sorting.ByRatingAsc:
                            query = query.OrderBy(x => x.Rating);
                            break;
                        case Sorting.ByRatingDesc:
                            query = query.OrderByDescending(x => x.Rating);
                            break;
                        case Sorting.ByOrdersAsc:
                            query = query.OrderBy(x => x.OrdersCount);
                            break;
                        case Sorting.ByOrdersDesc:
                            query = query.OrderByDescending(x => x.OrdersCount);
                            break;
                        case Sorting.ByProfitAsc:
                            query = query.OrderBy(x => x.TotalProfit);
                            break;
                        case Sorting.ByProfitDesc:
                            query = query.OrderByDescending(x => x.TotalProfit);
                            break;
                    }
                }
                else
                {
                    query = query.OrderByDescending(x => x.Rating);
                }

                var count = query.Count();

                if (filter.Skip.HasValue)
                    query = query.Skip(filter.Skip.Value);
                if (filter.Take.HasValue)
                    query = query.Take(filter.Take.Value);

                var investments = query.Select(x => x.ToInvestmentProgram()).ToList();
                return (investments, count);
            });
        }

        public OperationResult<InvestmentProgram> GetInvestment(Guid investmentId)
        {
            return InvokeOperations.InvokeOperation(() =>
            {
                var program = context.InvestmentPrograms
                                     .Include(x => x.ManagerAccount)
                                     .Include(x => x.Token)
                                     .Include(x => x.Periods)
                                     .First(x => x.Id == investmentId);

                return program.ToInvestmentProgram();
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

        public OperationResult<InvestorDashboard> GetInvestorDashboard(Guid investorUserId)
        {
            return InvokeOperations.InvokeOperation(() =>
            {
                var requests = context.InvestmentRequests
                                      .Include(x => x.InvestmentProgram)
                                      .Where(x => x.InvestorAccount.UserId == investorUserId)
                                      .ToList()
                                      .GroupBy(x => x.InvestmentProgram,
                                          (program, reqs) => new InvestorProgram
                                                             {
                                                                 InvestmentProgram = program.ToInvestmentShort(),
                                                                 Requests = reqs.Select(x => x.ToInvestmentRequest()).ToList()
                                                             })
                                      .ToList();

                var result = new InvestorDashboard
                             {
                                 Programs = requests
                             };

                return result;
            });
        }

        private OperationResult<string> UpdateInvestmentInIpfs(Guid investmentId)
        {
            var investmentProgram = context.InvestmentPrograms
                                           .Include(x => x.ManagerAccount)
                                           .ThenInclude(x => x.User)
                                           .ThenInclude(x => x.Profile)
                                           .Include(x => x.Token)
                                           .Include(x => x.Periods)
                                           .First(x => x.Id == investmentId);

            var json = JsonConvert.SerializeObject(investmentProgram.ToInvestmentProgram());

            return ipfsService.WriteIpfsText(json);
        }
    }
}
