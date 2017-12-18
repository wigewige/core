using System;
using GenesisVision.Core.Models;
using GenesisVision.Core.ViewModels.Investment;
using GenesisVision.Core.ViewModels.Manager;

namespace GenesisVision.Core.Services.Interfaces
{
    public interface ITrustManagementService
    {
        OperationResult<Guid> CreateInvestmentProgram(CreateInvestment investment);
    }
}
