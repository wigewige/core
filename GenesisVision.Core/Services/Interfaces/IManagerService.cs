using System;
using GenesisVision.Core.Models;
using GenesisVision.Core.ViewModels.Manager;

namespace GenesisVision.Core.Services.Interfaces
{
    public interface IManagerService
    {
        OperationResult<Guid> CreateManagerAccount(NewManager request);
        OperationResult<Guid> CreateManagerAccountRequest(NewManagerRequest request);
    }
}
