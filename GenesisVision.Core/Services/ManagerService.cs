using GenesisVision.Core.Helpers.Convertors;
using GenesisVision.Core.Models;
using GenesisVision.Core.Services.Interfaces;
using GenesisVision.Core.ViewModels.Manager;
using GenesisVision.DataModel;
using GenesisVision.DataModel.Enums;
using GenesisVision.DataModel.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using GenesisVision.Core.ViewModels.Broker;

namespace GenesisVision.Core.Services
{
    public class ManagerService : IManagerService
    {
        private readonly ApplicationDbContext context;
        private readonly IIpfsService ipfsService;
        private readonly ISmartContractService smartContractService;

        public ManagerService(ApplicationDbContext context, IIpfsService ipfsService, ISmartContractService smartContractService)
        {
            this.context = context;
            this.ipfsService = ipfsService;
            this.smartContractService = smartContractService;
        }

        public OperationResult<Guid> CreateNewInvestmentRequest(NewInvestmentRequest request)
        {
            return InvokeOperations.InvokeOperation(() =>
            {
                var req = new ManagerRequests
                          {
                              Id = Guid.NewGuid(),
                              UserId = request.UserId,
                              Date = DateTime.Now,
                              Type = ManagerRequestType.FromCabinet,
                              Status = ManagerRequestStatus.Created,
                              BrokerTradeServerId = request.BrokerTradeServerId,
                              DepositAmount = request.DepositAmount,
                              TradePlatformPassword = request.TradePlatformPassword,
                              TradePlatformCurrency = "USD",
                              TokenName = request.TokenName,
                              TokenSymbol = request.TokenSymbol,
                              Logo = request.Logo,
                              Description = request.Description,
                              DateFrom = request.DateFrom ?? DateTime.Now,
                              DateTo = request.DateTo,
                              Period = request.Period,
                              FeeSuccess = request.FeeSuccess,
                              FeeManagement = request.FeeManagement,
                              FeeEntrance = request.FeeEntrance,
                              InvestMaxAmount = request.InvestMaxAmount,
                              InvestMinAmount = request.InvestMinAmount
                          };

                var wallet = context.Wallets.First(x => x.UserId == request.UserId);
                wallet.Amount -= req.DepositAmount;

                var tx = new WalletTransactions
                         {
                             Id = Guid.NewGuid(),
                             Type = WalletTransactionsType.OpenProgram,
                             UserId = request.UserId,
                             Amount = request.DepositAmount,
                             Date = DateTime.Now
                         };

                context.Add(req);
                context.Add(tx);
                context.SaveChanges();

                return req.Id;
            });
        }

        public OperationResult<Guid> CreateManagerAccount(NewManager request)
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
                if (!inv.DateTo.HasValue || inv.DateTo < inv.DateFrom.AddDays(inv.Period))
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

        public OperationResult<List<ManagerRequest>> GetCreateAccountRequests(Guid brokerTradeServerId)
        {
            return InvokeOperations.InvokeOperation(() =>
            {
                var result = context.ManagerRequests
                                    .Include(x => x.User)
                                    .ThenInclude(x => x.Profile)
                                    .Where(x => x.BrokerTradeServerId == brokerTradeServerId && x.Status == ManagerRequestStatus.Created)
                                    .Select(x => x.ToManagerRequest())
                                    .ToList();
                return result;
            });
        }
        
        private OperationResult<string> UpdateInvestmentInIpfs(Guid investmentId)
        {
            var investmentProgram = context.InvestmentPrograms
                                           .Include(x => x.ManagerAccount)
                                           .Include(x => x.Token)
                                           .Include(x => x.Periods)
                                           .First(x => x.Id == investmentId);

            var json = JsonConvert.SerializeObject(investmentProgram.ToInvestmentProgram());

            return ipfsService.WriteIpfsText(json);
        }
    }
}
