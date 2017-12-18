using GenesisVision.Core.Data;
using GenesisVision.Core.Data.Models;
using GenesisVision.Core.Services.Interfaces;
using GenesisVision.Core.ViewModels.Manager;
using System;
using System.Linq;
using GenesisVision.Core.Models;

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
            var req = new ManagerAccountRequests
                      {
                          Id = Guid.NewGuid(),
                          Avatar = string.Empty,
                          BrokerTradeServerId = request.BrokerTradeServerId,
                          Currency = request.Currency,
                          Description = request.Description,
                          Name = request.Name,
                          UserId = request.UserId,
                          Date = DateTime.Now,
                          Type = ManagerRequestType.FromCabinet,
                          Status = ManagerRequestStatus.Created
                      };
            context.Add(req);
            context.SaveChanges();

            return OperationResult<Guid>.Ok(req.Id);
        }

        public OperationResult<Guid> CreateManagerAccount(NewManager request)
        {
            var managerRequest = context.ManagerRequests.First(x => x.Id == request.RequestId);

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

            return OperationResult<Guid>.Ok(manager.Id);
        }
    }
}
