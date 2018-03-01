using GenesisVision.Core.Helpers.Convertors;
using GenesisVision.Core.Models;
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
                            var wallet = context.Wallets.First(x => x.UserId == request.UserId && x.Currency == Currency.GVT);
                            wallet.Amount += request.Amount;

                            var tx = new WalletTransactions
                                     {
                                         Id = Guid.NewGuid(),
                                         Type = WalletTransactionsType.CancelInvestmentRequest,
                                         WalletId = wallet.Id,
                                         Amount = request.Amount,
                                         Date = DateTime.Now
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

        public OperationResult<(List<InvestmentProgram>, int)> GetInvestmentPrograms(InvestmentProgramsFilter filter, Guid? userId)
        {
            return InvokeOperations.InvokeOperation(() =>
            {
                var query = context.InvestmentPrograms
                                   .Include(x => x.ManagerAccount)
                                   .ThenInclude(x => x.ManagersAccountsTrades)
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
                
                var programs = query.Select(x => x.ToInvestmentProgram(userId)).ToList();

                return (programs, count);
            });
        }

        public OperationResult<InvestmentProgramDetails> GetInvestmentProgram(Guid investmentId, Guid? userId)
        {
            return InvokeOperations.InvokeOperation(() =>
            {
                var program = context.InvestmentPrograms
                                     .Include(x => x.ManagerAccount)
                                     .ThenInclude(x => x.ManagersAccountsTrades)
                                     .Include(x => x.ManagerAccount)
                                     .ThenInclude(x => x.User)
                                     .ThenInclude(x => x.Profile)
                                     .Include(x => x.InvestmentRequests)
                                     .Include(x => x.Periods)
                                     .First(x => x.Id == investmentId);
                
                return program.ToInvestmentProgramDetails(userId);
            });
        }

        public OperationResult<InvestorDashboard> GetInvestorDashboard(Guid investorUserId, Guid? userId)
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
                                      .Include(x => x.InvestmentProgram.Periods)
                                      .Where(x => x.InvestorAccount.UserId == investorUserId)
                                      .ToList()
                                      .GroupBy(x => x.InvestmentProgram, (program, reqs) => program.ToInvestmentProgramDashboard(userId))
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
                                             x.DateFrom < DateTime.Now &&
                                             (x.DateTo == null || x.DateTo > DateTime.Now))
                                 .Select(x => x.ToBrokerInvestmentProgram())
                                 .ToList();
                return res;
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
                               CurrentPeriod = investment.Periods.FirstOrDefault(x => x.Status == PeriodStatus.InProccess)?.ToPeriod()
                           };

                data.CanCloseCurrentPeriod = data.CurrentPeriod != null && data.CurrentPeriod.DateTo <= DateTime.Now;
                data.TokenHolders = context.Portfolios
                                           .Where(x => x.ManagerTokenId == investment.ManagerTokensId)
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

                statisticService.RecalculateStatisticForPeriod(currentPeriod.Id);
            });
        }

        public OperationResult SetPeriodStartValues(Guid investmentProgramId, decimal balance, decimal managerBalance, decimal managerShare)
        {
            return InvokeOperations.InvokeOperation(() =>
            {
                var nextPeriod = context.Periods
                                        .Include(x => x.InvestmentRequests)
                                        .First(x => x.Id == investmentProgramId && x.Status == PeriodStatus.Planned);

                nextPeriod.StartBalance = balance;
                nextPeriod.ManagerStartBalance = managerBalance;
                nextPeriod.ManagerStartShare = managerShare;

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
                                           .Include(x => x.Token)
                                           .Include(x => x.Periods)
                                           .Include(x => x.InvestmentRequests)
                                           .First(x => x.Id == investmentId);

            var json = JsonConvert.SerializeObject(investmentProgram.ToInvestmentProgram(null));

            return ipfsService.WriteIpfsText(json);
        }

        public OperationResult AccrueProfits(InvestmentProgramAccrual accrual)
        {
            return InvokeOperations.InvokeOperation(() =>
            {
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
                wallet.Amount -= totalProfit;

                var brokerTx = new WalletTransactions
                               {
                                   Id = Guid.NewGuid(),
                                   Type = WalletTransactionsType.ProfitFromProgram,
                                   Amount = -totalProfit,
                                   Date = DateTime.Now,
                                   WalletId = wallet.Id
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
                    investorWallet.Amount += acc.Amount;

                    var investorTx = new WalletTransactions
                                     {
                                         Id = Guid.NewGuid(),
                                         Type = WalletTransactionsType.ProfitFromProgram,
                                         Amount = acc.Amount,
                                         Date = DateTime.Now,
                                         WalletId = investorWallet.Id
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
                        Date = DateTime.Now
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
                                        .First(x => x.Id == investmentProgramId && x.Status == PeriodStatus.Planned);

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
                
                foreach (var request in nextPeriod.InvestmentRequests.OrderByDescending(x => x.Type).ThenBy(x => x.Date).Where(i => i.Status == InvestmentRequestStatus.New && i.UserId != investmentProgram.ManagerAccountId))
                {
                    request.Status = InvestmentRequestStatus.Executed;

                    var investor = context.InvestorAccounts
                                          .Include(x => x.Portfolios)
                                          .Include(x => x.User)
                                          .ThenInclude(x => x.Wallets)
                                          .First(x => x.UserId == request.UserId);

                    var portfolio = investor.Portfolios.FirstOrDefault(x => x.ManagerTokenId == investmentProgram.Token.Id);

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
                            gvtAmount = GVTUSDRate.Data / (investmentProgram.Token.FreeTokens * investmentProgram.Token.InitialPrice);
                            tokensAmount = investmentProgram.Token.FreeTokens;

                            var wallet = investor.User.Wallets.First(x => x.Currency == Currency.GVT);
                            wallet.Amount += request.Amount - gvtAmount;

                            var tx = new WalletTransactions
                            {
                                Id = Guid.NewGuid(),
                                Type = WalletTransactionsType.PartialInvestmentExecutionRefund,
                                WalletId = wallet.Id,
                                Amount = request.Amount - gvtAmount,
                                Date = DateTime.Now
                            };

                            context.Add(tx);
                        }

                        brokerBalanceChange += gvtAmount;
                        result.AccountBalanceChange += gvtAmount * GVTUSDRate.Data;

                        if (portfolio == null)
                        {
                            var newPortfolio = new Portfolios
                            {
                                InvestorAccountId = request.UserId,
                                ManagerTokenId = investmentProgram.Token.Id,
                                Amount = tokensAmount
                            };

                            context.Add(newPortfolio);
                        }
                        else
                        {
                            portfolio.Amount += tokensAmount;
                        }
                    }
                    else
                    {
                        var portfolioValue = portfolio.Amount * investmentProgram.Token.InitialPrice;

                        //ToDo: Actual amount to request
                        var amount = portfolioValue > request.Amount ? request.Amount : portfolioValue;

                        var amountInGVT = amount / GVTUSDRate.Data;

                        brokerBalanceChange -= amountInGVT;
                        result.AccountBalanceChange -= amount;

                        var tokensAmount = amount / investmentProgram.Token.InitialPrice;
                        portfolio.Amount -= tokensAmount;
                        investmentProgram.Token.FreeTokens += tokensAmount;


                        var wallet = investor.User.Wallets.First(x => x.Currency == Currency.GVT);
                        wallet.Amount += amountInGVT;

                        var investorTx = new WalletTransactions
                                         {
                                             Id = Guid.NewGuid(),
                                             Type = WalletTransactionsType.WithdrawFromProgram,
                                             WalletId = wallet.Id,
                                             Amount = amountInGVT,
                                             Date = DateTime.Now
                                         };

                        context.Add(investorTx);
                    }
                }

                foreach (var request in nextPeriod.InvestmentRequests.Where(i => i.Status == InvestmentRequestStatus.New && i.UserId == investmentProgram.ManagerAccountId))
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
                                      .First(x => x.Id == investmentProgram.ManagerAccountId);

                        var wallet = manager.Wallets.First(x => x.Currency == Currency.GVT);
                        wallet.Amount += amountInGVT;

                        var managerTx = new WalletTransactions
                                        {
                                            Id = Guid.NewGuid(),
                                            Type = WalletTransactionsType.WithdrawFromProgram,
                                            Amount = amountInGVT,
                                            Date = DateTime.Now,
                                            WalletId = wallet.Id
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
    }
}
