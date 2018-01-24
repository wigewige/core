using System;
using System.Collections.Generic;
using GenesisVision.Core.Models;
using GenesisVision.Core.ViewModels.Broker;
using GenesisVision.Core.ViewModels.Manager;

namespace GenesisVision.Core.Services.Interfaces
{
    public interface IManagerService
    {
        OperationResult<Guid> CreateManagerAccount(NewManager request);
        OperationResult<Guid> CreateNewInvestmentRequest(NewInvestmentRequest request);
        OperationResult<List<ManagerRequest>> GetCreateAccountRequests(Guid brokerTradeServerId);
    }
}
