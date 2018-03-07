using GenesisVision.Common.Models;
using GenesisVision.Common.Services.Interfaces;
using GenesisVision.Core.Helpers;
using GenesisVision.Core.Helpers.Convertors;
using GenesisVision.Core.Services.Interfaces;
using GenesisVision.Core.ViewModels.Broker;
using GenesisVision.Core.ViewModels.Investment;
using GenesisVision.DataModel;
using GenesisVision.DataModel.Enums;
using GenesisVision.DataModel.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
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
        private readonly IStatisticService statisticService;
        private readonly IRateService rateService;
        private readonly ILogger<ITrustManagementService> logger;

        public TrustManagementService(ApplicationDbContext context, IIpfsService ipfsService, ISmartContractService smartContractService, IStatisticService statisticService, IRateService rateService, ILogger<ITrustManagementService> logger)
        {
            this.context = context;
            this.ipfsService = ipfsService;
            this.smartContractService = smartContractService;
            this.statisticService = statisticService;
            this.rateService = rateService;
            this.logger = logger;
        }
        
        public OperationResult CloseInvestmentProgram(Guid invProgramId)
        {
            return InvokeOperations.InvokeOperation(() =>
            {
                var investment = context.InvestmentPrograms
                                        .Include(x => x.Periods)
                                        .ThenInclude(x => x.InvestmentRequests)
                                        .First(x => x.Id == invProgramId);

                investment.DateTo = DateTime.UtcNow;

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
                            var wallet = context.Wallets.First(x => x.UserId == request.UserId && x.Currency == Currency.GVT);
                            wallet.Amount += request.Amount;

                            var tx = new WalletTransactions
                                     {
                                         Id = Guid.NewGuid(),
                                         Type = WalletTransactionsType.CancelInvestmentRequest,
                                         WalletId = wallet.Id,
                                         Amount = request.Amount,
                                         Date = DateTime.UtcNow,
                                         InvestmentProgramtId = invProgramId
                                     };
                            context.Add(tx);
                        }
                        pendingInvests.ForEach(x => x.Status = InvestmentRequestStatus.Cancelled);
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
                                  RegistrationDate = DateTime.UtcNow,
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
                                TokenSymbol = managerRequest.TokenSymbol,
                                InitialPrice = 1,
                                FreeTokens = 1000
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
                              ManagerTokenId = token.Id,
                              Logo = managerRequest.Logo,
                              Rating = 0,
                          };
                var firstPeriod = new Periods
                                  {
                                      Id = Guid.NewGuid(),
                                      DateFrom = inv.DateFrom,
                                      DateTo = Constants.IsPeriodInMinutes
                                          ? inv.DateFrom.AddMinutes(inv.Period)
                                          : inv.DateFrom.AddDays(inv.Period),
                                      Status = PeriodStatus.InProccess,
                                      InvestmentProgramId = inv.Id,
                                      Number = 1,
                                      StartBalance = managerRequest.DepositInUsd,
                                      ManagerStartBalance = managerRequest.DepositInUsd,
                                      ManagerStartShare = 1
                                  };
                if (!inv.DateTo.HasValue || inv.DateTo > inv.DateFrom.AddDays(inv.Period))
                {
                    var plannedPeriod = new Periods
                                        {
                                            Id = Guid.NewGuid(),
                                            DateFrom = firstPeriod.DateTo,
                                            DateTo = Constants.IsPeriodInMinutes
                                                ? firstPeriod.DateTo.AddMinutes(inv.Period)
                                                : firstPeriod.DateTo.AddDays(inv.Period),
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
                var investor = context.Users
                                      .Include(x => x.Wallets)
                                      .First(x => x.Id == model.UserId);
                var wallet = investor.Wallets.First(x => x.Currency == Currency.GVT);
                var lastPeriod = context.Periods
                                        .Where(x => x.InvestmentProgramId == model.InvestmentProgramId)
                                        .OrderByDescending(x => x.Number)
                                        .First();

                var tx = new WalletTransactions
                         {
                             Id = Guid.NewGuid(),
                             Type = WalletTransactionsType.InvestToProgram,
                             WalletId = wallet.Id,
                             Amount = model.Amount,
                             Date = DateTime.UtcNow,
                             InvestmentProgramtId = model.InvestmentProgramId
                         };

                var invRequest = new InvestmentRequests
                                 {
                                     Id = Guid.NewGuid(),
                                     UserId = model.UserId,
                                     Amount = model.Amount,
                                     Date = DateTime.UtcNow,
                                     InvestmentProgramtId = model.InvestmentProgramId,
                                     Status = InvestmentRequestStatus.New,
                                     Type = InvestmentRequestType.Invest,
                                     PeriodId = lastPeriod.Id,
                                     WalletTransactionId = tx.Id
                                 };

                wallet.Amount -= model.Amount;
                
                context.Add(invRequest);
                context.Add(tx);
                context.SaveChanges();
            });
        }

        public OperationResult RequestForWithdraw(Invest model)
        {
            return InvokeOperations.InvokeOperation(() =>
            {
                var investor = context.Users
                                      .First(x => x.Id == model.UserId);

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
                                     Date = DateTime.UtcNow,
                                     InvestmentProgramtId = model.InvestmentProgramId,
                                     Status = InvestmentRequestStatus.New,
                                     Type = InvestmentRequestType.Withdrawal,
                                     PeriodId = period.Id
                                 };

                context.Add(invRequest);
                context.SaveChanges();
            });
        }

        public OperationResult<(List<InvestmentProgram>, int)> GetInvestmentPrograms(InvestmentProgramsFilter filter, Guid? userId, UserType? userType)
        {
            return InvokeOperations.InvokeOperation(() =>
            {
                var query = context.InvestmentPrograms
                                   .Include(x => x.ManagerAccount)
                                   .ThenInclude(x => x.ManagersAccountsTrades)
                                   .Include(x => x.ManagerAccount.ManagersAccountsStatistics)
                                   .Include(x => x.Token)
                                   .Include(x => x.InvestmentRequests)
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
                            query = query.OrderBy(x => x.ManagerAccount.OrdersCount);
                            break;
                        case Sorting.ByOrdersDesc:
                            query = query.OrderByDescending(x => x.ManagerAccount.OrdersCount);
                            break;
                        case Sorting.ByProfitAsc:
                            query = query.OrderBy(x => x.ManagerAccount.ProfitTotal);
                            break;
                        case Sorting.ByProfitDesc:
                            query = query.OrderByDescending(x => x.ManagerAccount.ProfitTotal);
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
                
                var programs = query.Select(x => x.ToInvestmentProgram(userId, userType)).ToList();

                return (programs, count);
            });
        }

        public OperationResult<InvestmentProgramDetails> GetInvestmentProgram(Guid investmentId, Guid? userId, UserType? userType)
        {
            return InvokeOperations.InvokeOperation(() =>
            {
                var program = context.InvestmentPrograms
                                     .Include(x => x.Token)
                                     .ThenInclude(x => x.InvestorTokens)
                                     .ThenInclude(x => x.InvestorAccount)
                                     .Include(x => x.ManagerAccount)
                                     .ThenInclude(x => x.ManagersAccountsTrades)
                                     .Include(x => x.ManagerAccount.ManagersAccountsStatistics)
                                     .Include(x => x.ManagerAccount)
                                     .ThenInclude(x => x.User)
                                     .ThenInclude(x => x.Profile)
                                     .Include(x => x.InvestmentRequests)
                                     .Include(x => x.Periods)
                                     .First(x => x.Id == investmentId);
                
                return program.ToInvestmentProgramDetails(userId, userType);
            });
        }

        public OperationResult<InvestorDashboard> GetInvestorDashboard(Guid investorUserId, Guid? userId, UserType? userType)
        {
            return InvokeOperations.InvokeOperation(() =>
            {
                var requests = context.InvestmentRequests
                                      .Include(x => x.InvestmentProgram)
                                      .ThenInclude(x => x.ManagerAccount)
                                      .ThenInclude(x => x.ManagersAccountsTrades)
                                      .Include(x => x.InvestmentProgram)
                                      .ThenInclude(x => x.ManagerAccount)
                                      .ThenInclude(x => x.User)
                                      .ThenInclude(x => x.Profile)
                                      .Include(x => x.InvestmentProgram.Token)
                                      .Include(x => x.InvestmentProgram.ManagerAccount.ManagersAccountsStatistics)
                                      .Include(x => x.InvestmentProgram.Periods)
                                      .Include(x => x.InvestmentProgram.Token)
                                      .ThenInclude(x => x.InvestorTokens)
                                      .ThenInclude(x => x.InvestorAccount)
                                      .Where(x => x.InvestorAccount.UserId == investorUserId)
                                      .ToList()
                                      .GroupBy(x => x.InvestmentProgram, (program, reqs) => program.ToInvestmentProgramDashboard(userId, userType))
                                      .ToList();

                var result = new InvestorDashboard {InvestmentPrograms = requests};

                return result;
            });
        }

        public OperationResult<List<BrokerInvestmentProgram>> GetBrokerInvestmentsInitData(Guid brokerTradeServerId)
        {
            return InvokeOperations.InvokeOperation(() =>
            {
                var res = context.InvestmentPrograms
                                 .Include(x => x.ManagerAccount)
                                 .Include(x => x.Periods)
                                 .Where(x => x.ManagerAccount.BrokerTradeServerId == brokerTradeServerId &&
                                             x.IsEnabled &&
                                             x.DateFrom < DateTime.UtcNow &&
                                             (x.DateTo == null || x.DateTo > DateTime.UtcNow))
                                 .Select(x => x.ToBrokerInvestmentProgram())
                                 .ToList();
                return res;
            });
        }

        public OperationResult<ClosePeriodData> GetClosingPeriodData(Guid investmentProgramId)
        {
            return InvokeOperations.InvokeOperation(() =>
            {
                var program = context.InvestmentPrograms
                                     .Include(x => x.Periods)
                                     .ThenInclude(x => x.InvestmentRequests)
                                     .First(x => x.Id == investmentProgramId);

                var data = new ClosePeriodData
                           {
                               CurrentPeriod = program.Periods.FirstOrDefault(x => x.Status == PeriodStatus.InProccess)?.ToPeriod()
                           };

                data.CanCloseCurrentPeriod = data.CurrentPeriod != null && data.CurrentPeriod.DateTo <= DateTime.UtcNow;
                data.TokenHolders = context.InvestorTokens
                                           .Where(x => x.ManagerTokenId == program.ManagerTokenId)
                                           .Select(x => new InvestorAmount
                                                        {
                                                            InvestorId = x.InvestorAccountId,
                                                            Amount = x.Amount
                                                        })
                                           .ToList();

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
                
                var nextPeriod = investment.Periods.FirstOrDefault(x => x.Status == PeriodStatus.Planned);
                if (nextPeriod != null)
                {
                    nextPeriod.Status = PeriodStatus.InProccess;
                }

                if (!investment.DateTo.HasValue || DateTime.UtcNow < investment.DateTo.Value)
                {
                    var newPeriod = new Periods
                                    {
                                        Id = Guid.NewGuid(),
                                        DateFrom = nextPeriod?.DateTo ?? DateTime.UtcNow,
                                        DateTo = Constants.IsPeriodInMinutes
                                            ? (nextPeriod?.DateTo ?? DateTime.UtcNow).AddMinutes(investment.Period)
                                            : (nextPeriod?.DateTo ?? DateTime.UtcNow).AddDays(investment.Period),
                                        InvestmentProgramId = investmentProgramId,
                                        Number = investment.Periods.Max(x => x.Number) + 1,
                                        Status = nextPeriod == null ? PeriodStatus.InProccess : PeriodStatus.Planned
                                    };
                    context.Add(newPeriod);
                }

                if (investment.DateTo.HasValue && investment.DateTo < DateTime.UtcNow)
                {
                    investment.IsEnabled = false;
                }

                context.SaveChanges();

                statisticService.RecalculateStatisticForPeriod(currentPeriod.Id);
            });
        }

        public OperationResult SetPeriodStartValues(StartValues values)
        {
            return InvokeOperations.InvokeOperation(() =>
            {
                var nextPeriod = context.Periods.First(x => x.InvestmentProgramId == values.InvestmentProgramId && x.Status == PeriodStatus.Planned);

                nextPeriod.StartBalance = values.Balance;
                nextPeriod.ManagerStartBalance = values.ManagerBalance;
                nextPeriod.ManagerStartShare = values.ManagerShare;

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
        
        private OperationResult<string> UpdateInvestmentInIpfs(Guid investmentId)
        {
            var investmentProgram = context.InvestmentPrograms
                                           .Include(x => x.ManagerAccount)
                                           .ThenInclude(x => x.ManagersAccountsTrades)
                                           .Include(x => x.ManagerAccount.ManagersAccountsStatistics)
                                           .Include(x => x.Token)
                                           .Include(x => x.Periods)
                                           .Include(x => x.InvestmentRequests)
                                           .First(x => x.Id == investmentId);

            var json = JsonConvert.SerializeObject(investmentProgram.ToInvestmentProgram(null, null));

            return ipfsService.WriteIpfsText(json);
        }

        public OperationResult AccrueProfits(InvestmentProgramAccrual accrual)
        {
            return InvokeOperations.InvokeOperation(() =>
            {
                var GVTUSDRate = rateService.GetRate(Currency.GVT, Currency.USD);
                if (!GVTUSDRate.IsSuccess)
                    throw new Exception("Error at GetRate: " + string.Join(", ", GVTUSDRate.Errors));

                var lastPeriod = context.Periods
                                        .Where(x => x.InvestmentProgramId == accrual.InvestmentProgramId)
                                        .OrderByDescending(x => x.Number)
                                        .First();

                var investmentProgram = context.InvestmentPrograms
                                               .Include(x => x.ManagerAccount)
                                               .ThenInclude(x => x.BrokerTradeServer)
                                               .ThenInclude(x => x.Broker)
                                               .ThenInclude(x => x.User)
                                               .ThenInclude(x => x.Wallets)
                                               .First(x => x.Id == accrual.InvestmentProgramId);

                var totalProfit = accrual.Accruals.Sum(a => a.Amount);

                var wallet = investmentProgram.ManagerAccount.BrokerTradeServer.Broker.User.Wallets.First(x => x.Currency == Currency.GVT);
                var brokerAmount = totalProfit / GVTUSDRate.Data;
                wallet.Amount -= brokerAmount;

                var brokerTx = new WalletTransactions
                               {
                                   Id = Guid.NewGuid(),
                                   Type = WalletTransactionsType.ProfitFromProgram,
                                   Amount = -brokerAmount,
                                   Date = DateTime.UtcNow,
                                   WalletId = wallet.Id,
                                   InvestmentProgramtId = accrual.InvestmentProgramId
                               };

                var brokerProfitDistribution = new ProfitDistributionTransactions
                                               {
                                                   PeriodId = lastPeriod.Id,
                                                   WalletTransactionId = brokerTx.Id
                                               };

                context.Add(brokerTx);
                context.Add(brokerProfitDistribution);

                foreach (var acc in accrual.Accruals)
                {
                    var investor = context.InvestorAccounts
                                          .Include(x => x.User)
                                          .ThenInclude(x => x.Wallets)
                                          .First(x => x.UserId == acc.InvestorId);

                    var investorWallet = investor.User.Wallets.First(x => x.Currency == Currency.GVT);
                    var investorAmount = acc.Amount / GVTUSDRate.Data;
                    investorWallet.Amount += investorAmount;

                    var investorTx = new WalletTransactions
                                     {
                                         Id = Guid.NewGuid(),
                                         Type = WalletTransactionsType.ProfitFromProgram,
                                         Amount = investorAmount,
                                         Date = DateTime.UtcNow,
                                         WalletId = investorWallet.Id,
                                         InvestmentProgramtId = accrual.InvestmentProgramId
                                     };

                    var investorProfitDistribution = new ProfitDistributionTransactions
                                                     {
                                                         PeriodId = lastPeriod.Id,
                                                         WalletTransactionId = investorTx.Id
                                                     };

                    context.Add(investorTx);
                    context.Add(investorProfitDistribution);
                }

                //ToDo: update manager's next period balance

                context.SaveChanges();
            });
        }

        public OperationResult CancelInvestmentRequest(Guid requestId)
        {
            return InvokeOperations.InvokeOperation(() =>
            {
                var investmentRequest = context.InvestmentRequests
                                               .Include(x => x.User)
                                               .ThenInclude(x => x.Wallets)
                                               .First(x => x.Id == requestId && x.Status == InvestmentRequestStatus.New);

                investmentRequest.Status = InvestmentRequestStatus.Cancelled;

                if (investmentRequest.Type == InvestmentRequestType.Invest)
                {
                    var investor = investmentRequest.User;
                    var wallet = investor.Wallets.First(x => x.Currency == Currency.GVT);

                    var tx = new WalletTransactions
                             {
                                 Id = Guid.NewGuid(),
                                 Type = WalletTransactionsType.CancelInvestmentRequest,
                                 WalletId = wallet.Id,
                                 Amount = investmentRequest.Amount,
                                 Date = DateTime.UtcNow,
                                 InvestmentProgramtId = investmentRequest.InvestmentProgramtId
                             };

                    context.Add(tx);

                    wallet.Amount += investmentRequest.Amount;
                }

                context.SaveChanges();
            });
        }

        public OperationResult<BalanceChange> ProcessInvestmentRequests(Guid investmentProgramId)
        {
            return InvokeOperations.InvokeOperation(() =>
            {
                var result = new BalanceChange();
                decimal brokerBalanceChange = 0;

                //Todo: manager's threshold amount
                var managerThresholdAmount = 1000;

                var GVTUSDRate = rateService.GetRate(Currency.GVT, Currency.USD);
                if (!GVTUSDRate.IsSuccess)
                    throw new Exception("Error at GetRate: " + string.Join(", ", GVTUSDRate.Errors));

                var nextPeriod = context.Periods
                                        .Include(x => x.InvestmentRequests)
                                        .First(x => x.InvestmentProgramId == investmentProgramId && x.Status == PeriodStatus.Planned);

                var investmentProgram = context.InvestmentPrograms
                                               .Include(x => x.Token)
                                               .Include(x => x.ManagerAccount)
                                               .ThenInclude(x => x.BrokerTradeServer)
                                               .ThenInclude(x => x.Broker)
                                               .ThenInclude(x => x.User)
                                               .ThenInclude(x => x.Wallets)
                                               .First(x => x.Id == investmentProgramId);

                var brokerWalletId = investmentProgram.ManagerAccount.BrokerTradeServer.Broker.User.Id;
                var GVTToManagerTokenRate = GVTUSDRate.Data / investmentProgram.Token.InitialPrice;
                
                foreach (var request in nextPeriod.InvestmentRequests
                                                  .OrderByDescending(x => x.Type)
                                                  .ThenBy(x => x.Date)
                                                  .Where(x => x.Status == InvestmentRequestStatus.New && x.UserId != investmentProgram.ManagerAccount.UserId))
                {
                    request.Status = InvestmentRequestStatus.Executed;

                    var investor = context.InvestorAccounts
                                          .Include(x => x.InvestorTokens)
                                          .Include(x => x.User)
                                          .ThenInclude(x => x.Wallets)
                                          .First(x => x.UserId == request.UserId);

                    var investorTokens = investor.InvestorTokens.FirstOrDefault(x => x.ManagerTokenId == investmentProgram.Token.Id);

                    if (request.Type == InvestmentRequestType.Invest)
                    {
                        //ToDo: Actual value in manager's currency to request
                        
                        var tokensAmount = request.Amount * GVTToManagerTokenRate;
                        var gvtAmount = request.Amount;

                        if (investmentProgram.Token.FreeTokens == 0)
                        {
                            //ToDo: separate refund transaction?
                            CancelInvestmentRequest(request.Id);
                            continue;
                        }
                        if (tokensAmount > investmentProgram.Token.FreeTokens)
                        {
                            gvtAmount =  investmentProgram.Token.FreeTokens * investmentProgram.Token.InitialPrice / GVTUSDRate.Data;
                            tokensAmount = investmentProgram.Token.FreeTokens;

                            var wallet = investor.User.Wallets.First(x => x.Currency == Currency.GVT);
                            wallet.Amount += request.Amount - gvtAmount;

                            var tx = new WalletTransactions
                            {
                                Id = Guid.NewGuid(),
                                Type = WalletTransactionsType.PartialInvestmentExecutionRefund,
                                WalletId = wallet.Id,
                                Amount = request.Amount - gvtAmount,
                                Date = DateTime.UtcNow,
                                InvestmentProgramtId = investmentProgramId
                            };

                            context.Add(tx);
                        }

                        brokerBalanceChange += gvtAmount;
                        result.AccountBalanceChange += gvtAmount * GVTUSDRate.Data;

                        if (investorTokens == null)
                        {
                            var newPortfolio = new InvestorTokens
                                               {
                                                   Id = Guid.NewGuid(),
                                                   InvestorAccountId = request.UserId,
                                                   ManagerTokenId = investmentProgram.Token.Id,
                                                   Amount = tokensAmount
                                               };
                            context.Add(newPortfolio);
                        }
                        else
                        {
                            investorTokens.Amount += tokensAmount;
                        }

                        investmentProgram.Token.FreeTokens -= tokensAmount;
                    }
                    else
                    {
                        var investorTokensValue = investorTokens.Amount * investmentProgram.Token.InitialPrice;

                        //ToDo: Actual amount to request
                        var amount = investorTokensValue > request.Amount ? request.Amount : investorTokensValue;

                        var amountInGVT = amount / GVTUSDRate.Data;

                        brokerBalanceChange -= amountInGVT;
                        result.AccountBalanceChange -= amount;

                        var tokensAmount = amount / investmentProgram.Token.InitialPrice;
                        investorTokens.Amount -= tokensAmount;
                        investmentProgram.Token.FreeTokens += tokensAmount;


                        var wallet = investor.User.Wallets.First(x => x.Currency == Currency.GVT);
                        wallet.Amount += amountInGVT;

                        var investorTx = new WalletTransactions
                                         {
                                             Id = Guid.NewGuid(),
                                             Type = WalletTransactionsType.WithdrawFromProgram,
                                             WalletId = wallet.Id,
                                             Amount = amountInGVT,
                                             Date = DateTime.UtcNow,
                                             InvestmentProgramtId = investmentProgramId
                                         };

                        context.Add(investorTx);
                    }
                }

                foreach (var request in nextPeriod.InvestmentRequests
                                                  .Where(i => i.Status == InvestmentRequestStatus.New && i.UserId == investmentProgram.ManagerAccount.UserId))
                {
                    request.Status = InvestmentRequestStatus.Executed;

                    if (request.Type == InvestmentRequestType.Invest)
                    {
                        brokerBalanceChange += request.Amount;

                        result.AccountBalanceChange += request.Amount * GVTUSDRate.Data;
                        result.ManagerBalanceChange += request.Amount * GVTUSDRate.Data;
                    }
                    else
                    {
                        var amount = nextPeriod.ManagerStartBalance > request.Amount + managerThresholdAmount
                            ? request.Amount
                            : nextPeriod.ManagerStartBalance - managerThresholdAmount;

                        var amountInGVT = amount / GVTUSDRate.Data;

                        brokerBalanceChange -= amountInGVT;

                        result.AccountBalanceChange -= amount;
                        result.ManagerBalanceChange -= amount;

                        var manager = context.Users
                                             .Include(x => x.Wallets)
                                             .First(x => x.Id == investmentProgram.ManagerAccount.UserId);

                        var wallet = manager.Wallets.First(x => x.Currency == Currency.GVT);
                        wallet.Amount += amountInGVT;

                        var managerTx = new WalletTransactions
                                        {
                                            Id = Guid.NewGuid(),
                                            Type = WalletTransactionsType.WithdrawFromProgram,
                                            Amount = amountInGVT,
                                            Date = DateTime.UtcNow,
                                            WalletId = wallet.Id,
                                            InvestmentProgramtId = investmentProgramId
                                        };

                        context.Add(managerTx);
                    }
                }

                //ToDo: Transaction record?
                investmentProgram.ManagerAccount.BrokerTradeServer.Broker.User.Wallets.First(x => x.Currency == Currency.GVT).Amount += brokerBalanceChange;

                context.SaveChanges();

                return result;
            });
        }

        public OperationResult ReevaluateManagerToken(Guid investmentProgramId, decimal investorLossShare)
        {
            return InvokeOperations.InvokeOperation(() =>
            {
                var managerToken = context.ManagerTokens
                                          .Include(x => x.InvestmentProgram)
                                          .First(x => x.InvestmentProgram.Id == investmentProgramId);

                managerToken.InitialPrice -= managerToken.InitialPrice * investorLossShare;
                managerToken.FreeTokens = managerToken.FreeTokens / (1 - investorLossShare);

                context.SaveChanges();
            });
        }

        public OperationResult<(List<InvestmentProgramRequest>, int)> GetInvestmentProgramRequests(InvestmentProgramRequestsFilter filter, Guid userId)
        {
            return InvokeOperations.InvokeOperation(() =>
            {
                var query = context.InvestmentRequests
                                   .Where(x => x.InvestmentProgramtId == filter.InvestmentProgramId &&
                                               x.UserId == userId);

                if (filter.Status.HasValue)
                    query = query.Where(x => x.Status == filter.Status.Value);
                if (filter.Type.HasValue)
                    query = query.Where(x => x.Type == filter.Type.Value);
                if (filter.DateFrom.HasValue)
                    query = query.Where(x => x.Date >= filter.DateFrom.Value);
                if (filter.DateTo.HasValue)
                    query = query.Where(x => x.Date < filter.DateTo.Value);

                var count = query.Count();

                if (filter.Skip.HasValue)
                    query = query.Skip(filter.Skip.Value);
                if (filter.Take.HasValue)
                    query = query.Take(filter.Take.Value);

                var programs = query.Select(x => x.ToInvestmentRequest()).ToList();

                return (programs, count);
            });
        }

        public OperationResult UpdateManagerHistoryIpfsHash(ManagerHistoryIpfsHash data)
        {
            return InvokeOperations.InvokeOperation(() =>
            {
                foreach (var manager in data.ManagersHashes)
                {
                    var managerData = context.ManagersAccounts.FirstOrDefault(x => x.Id == manager.ManagerId);
                    if (managerData == null)
                        continue;

                    managerData.TradeIpfsHash = manager.IpfsHash;
                }

                if (data.ManagersHashes.Any())
                    context.SaveChanges();
            });
        }

        public OperationResult ProcessClosingProgram(Guid investmentProgramId, decimal managerBalance)
        {
            return InvokeOperations.InvokeOperation(() =>
            {
                decimal brokerBalanceChange = 0;

                var USDGVTRate = rateService.GetRate(Currency.USD, Currency.GVT);
                if (!USDGVTRate.IsSuccess)
                    throw new Exception("Error at GetRate: " + string.Join(", ", USDGVTRate.Errors));

                var investmentProgram = context.InvestmentPrograms
                                               .Include(x => x.Token)
                                               .ThenInclude(x => x.InvestorTokens)
                                               .Include(x => x.ManagerAccount)
                                               .ThenInclude(x => x.BrokerTradeServer)
                                               .ThenInclude(x => x.Broker)
                                               .ThenInclude(x => x.User)
                                               .ThenInclude(x => x.Wallets)
                                               .First(x => x.Id == investmentProgramId);

                var tokenGVTRate = investmentProgram.Token.InitialPrice * USDGVTRate.Data;

                //Process investors
                foreach (var holder in investmentProgram.Token.InvestorTokens)
                {
                    var gvtAmount = holder.Amount * tokenGVTRate;

                    var investor = context.InvestorAccounts
                                          .Include(x => x.User)
                                          .ThenInclude(x => x.Wallets)
                                          .First(x => x.UserId == holder.InvestorAccountId);

                    var wallet = investor.User.Wallets.First(x => x.Currency == Currency.GVT);
                    wallet.Amount += gvtAmount;

                    var investorTx = new WalletTransactions
                    {
                        Id = Guid.NewGuid(),
                        Type = WalletTransactionsType.ClosingProgramRefund,
                        WalletId = wallet.Id,
                        Amount = gvtAmount,
                        Date = DateTime.UtcNow,
                        InvestmentProgramtId = investmentProgramId
                    };

                    context.Add(investorTx);

                    brokerBalanceChange += gvtAmount;

                    holder.Amount = 0;
                }

                //Process manager
                var managerAmountInGVT = managerBalance * USDGVTRate.Data;

                var manager = context.Users
                                     .Include(x => x.Wallets)
                                     .First(x => x.Id == investmentProgram.ManagerAccountId);

                var managerWallet = manager.Wallets.First(x => x.Currency == Currency.GVT);
                managerWallet.Amount += managerAmountInGVT;

                var managerTx = new WalletTransactions
                {
                    Id = Guid.NewGuid(),
                    Type = WalletTransactionsType.ClosingProgramRefund,
                    WalletId = managerWallet.Id,
                    Amount = managerAmountInGVT,
                    Date = DateTime.UtcNow,
                    InvestmentProgramtId = investmentProgramId
                };

                context.Add(managerTx);

                brokerBalanceChange += managerAmountInGVT;

                //Process broker
                var brokerWallet = investmentProgram.ManagerAccount.BrokerTradeServer.Broker.User.Wallets.First(x => x.Currency == Currency.GVT);
                brokerWallet.Amount -= brokerBalanceChange;

                var brokerTx = new WalletTransactions
                {
                    Id = Guid.NewGuid(),
                    Type = WalletTransactionsType.ClosingProgramRefund,
                    WalletId = brokerWallet.Id,
                    Amount = managerAmountInGVT,
                    Date = DateTime.UtcNow,
                    InvestmentProgramtId = investmentProgramId
                };

                context.Add(brokerTx);

                context.SaveChanges();
                
                ClosePeriod(investmentProgramId);
            });
        }
    }
}
