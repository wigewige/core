using GenesisVision.Core.Helpers;
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
                context.Add(req);
                context.SaveChanges();

                return req.Id;
            });
        }

        public OperationResult<Guid> CreateManagerAccount(NewManager request)
        {
            return InvokeOperations.InvokeOperation(() =>
            {
                var managerRequest = context.ManagerRequests.First(x => x.Id == request.RequestId && x.Status == ManagerRequestStatus.Created);

                var manager = new ManagerAccounts
                              {
                                  Id = Guid.NewGuid(),
                                  Avatar = managerRequest.Avatar,
                                  BrokerTradeServerId = managerRequest.BrokerTradeServerId,
                                  Currency = managerRequest.Currency,
                                  TokenName = managerRequest.TokenName,
                                  TokenSymbol = managerRequest.TokenSymbol,
                                  Description = managerRequest.Description,
                                  Login = request.Login,
                                  Name = managerRequest.Name,
                                  IsEnabled = true,
                                  Rating = 0,
                                  UserId = managerRequest.UserId,
                                  RegistrationDate = DateTime.Now
                              };
                context.Add(manager);
                managerRequest.Status = ManagerRequestStatus.Processed;
                context.SaveChanges();

                var blockchainUpdate =
                    smartContractService.RegisterManager(manager.TokenName, manager.TokenSymbol, manager.Id.ToString(), manager.Login, 
                    manager.BrokerTradeServerId.ToString(), 0, 0); // TODO fill fees

                if (blockchainUpdate.IsSuccess)
                {
                    manager.Confirmed = true;
                    context.SaveChanges();

                    var ipfsUpdate = UpdateManagerAccountInIpfs(manager.Id);
                    if (ipfsUpdate.IsSuccess)
                    {
                        manager.IpfsHash = ipfsUpdate.Data;
                        context.SaveChanges();
                    }
                }

                return manager.Id;
            });
        }

        public OperationResult UpdateManagerAccount(UpdateManagerAccount account)
        {
            return InvokeOperations.InvokeOperation(() =>
            {
                var manager = context.ManagersAccounts.First(x => x.Id == account.ManagerAccountId);

                manager.Avatar = account.Avatar;
                manager.Name = account.Name;
                manager.Description = account.Description;

                context.SaveChanges();

                var ipfsUpdate = UpdateManagerAccountInIpfs(account.ManagerAccountId);
                if (ipfsUpdate.IsSuccess)
                {
                    manager.IpfsHash = ipfsUpdate.Data;
                    context.SaveChanges();
                }
            });
        }

        public OperationResult<List<ManagerRequest>> GetNewRequests(Guid brokerTradeServerId)
        {
            return InvokeOperations.InvokeOperation(() =>
            {
                var result = context.ManagerRequests
                                    .Where(x => x.BrokerTradeServerId == brokerTradeServerId && x.Status == ManagerRequestStatus.Created)
                                    .Select(x => x.ToManagerRequest())
                                    .ToList();
                return result;
            });
        }

        public OperationResult<ManagerAccount> GetManagerDetails(Guid managerId)
        {
            return InvokeOperations.InvokeOperation(() =>
            {
                var manager = context.ManagersAccounts
                                     .Include(x => x.BrokerTradeServer)
                                     .ThenInclude(x => x.Broker)
                                     .First(x => x.Id == managerId);
                return manager.ToManagerAccount();
            });
        }

        public OperationResult<(List<ManagerAccount>, int)> GetManagersDetails(ManagersFilter filter)
        {
            return InvokeOperations.InvokeOperation(() =>
            {
                var query = context.ManagersAccounts
                                   .Include(x => x.BrokerTradeServer)
                                   .ThenInclude(x => x.Broker)
                                   .AsQueryable();

                if (!string.IsNullOrEmpty(filter.Name))
                {
                    var str = filter.Name.Trim().ToLower();
                    query = query.Where(x => x.Name.ToLower().Contains(str) ||
                                             x.Description.ToLower().Contains(str) ||
                                             x.Login.ToLower().Contains(str) ||
                                             x.Id.ToString().ToLower().Contains(str));
                }
                if (!string.IsNullOrEmpty(filter.BrokerName))
                {
                    var str = filter.BrokerName.Trim().ToLower();
                    query = query.Where(x => x.BrokerTradeServer.Broker.Name.ToLower().Contains(str) ||
                                             x.BrokerTradeServer.Broker.Description.ToLower().Contains(str) ||
                                             x.BrokerTradeServer.Broker.Id.ToString().ToLower().Contains(str));
                }
                if (!string.IsNullOrEmpty(filter.BrokerTradeServerName))
                {
                    var str = filter.BrokerTradeServerName.Trim().ToLower();
                    query = query.Where(x => x.BrokerTradeServer.Name.ToLower().Contains(str) ||
                                             x.BrokerTradeServer.Host.ToLower().Contains(str) ||
                                             x.BrokerTradeServer.Id.ToString().ToLower().Contains(str));
                }
                if (filter.BrokerTradeServerType.HasValue)
                    query = query.Where(x => x.BrokerTradeServer.Type == filter.BrokerTradeServerType.Value);

                var count = query.Count();

                if (filter.Skip.HasValue)
                    query = query.Skip(filter.Skip.Value);
                if (filter.Take.HasValue)
                    query = query.Take(filter.Take.Value);

                var managers = query.Select(x => x.ToManagerAccount()).ToList();
                return (managers, count);
            });
        }

        public OperationResult<List<ManagerAccount>> GetUserManagersAccounts(Guid userId)
        {
            return InvokeOperations.InvokeOperation(() =>
            {
                var managers = context.ManagersAccounts
                                      .Include(x => x.BrokerTradeServer)
                                      .ThenInclude(x => x.Broker)
                                      .Where(x => x.UserId == userId)
                                      .Select(x => x.ToManagerAccount())
                                      .ToList();
                return managers;
            });
        }

        private OperationResult<string> UpdateManagerAccountInIpfs(Guid managerId)
        {
            var account = GetManagerDetails(managerId);
            if (!account.IsSuccess)
                return OperationResult<string>.Failed();

            var json = JsonConvert.SerializeObject(account.Data);

            return ipfsService.WriteIpfsText(json);
        }
    }
}
