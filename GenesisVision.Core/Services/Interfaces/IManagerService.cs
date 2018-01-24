using GenesisVision.Core.Models;
using GenesisVision.Core.ViewModels.Manager;
using System;
using System.Collections.Generic;

namespace GenesisVision.Core.Services.Interfaces
{
    public interface IManagerService
    {
        OperationResult<Guid> CreateNewInvestmentRequest(NewInvestmentRequest request);
        OperationResult<List<ManagerRequest>> GetCreateAccountRequests(Guid brokerTradeServerId);
    }
}
