using GenesisVision.Core.Data;
using GenesisVision.Core.Data.Models;
using GenesisVision.Core.Helpers;
using GenesisVision.Core.Models;
using GenesisVision.Core.Services.Interfaces;
using GenesisVision.Core.ViewModels.Manager;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GenesisVision.Core.Services
{
    public class ManagerService : IManagerService
    {
        private readonly ApplicationDbContext context;

        public ManagerService(ApplicationDbContext context)
        {
            this.context = context;
        }

        public OperationResult<Guid> CreateManagerAccountRequest(NewManagerRequest request)
        {
            return InvokeOperations.InvokeOperation(() =>
            {
                Guid userId;
                if (request.UserId.HasValue)
                {
                    userId = request.UserId.Value;
                }
                else
                {
                    // todo: create new default AspNetUser
                    throw new Exception("User does not exist");
                }

                var req = new ManagerAccountRequests
                          {
                              Id = Guid.NewGuid(),
                              Avatar = string.Empty,
                              BrokerTradeServerId = request.BrokerTradeServerId,
                              Currency = request.Currency,
                              Description = request.Description,
                              Name = request.Name,
                              UserId = userId,
                              Date = DateTime.Now,
                              Type = ManagerRequestType.FromCabinet,
                              Status = ManagerRequestStatus.Created
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
                return manager.Id;
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
    }
}
