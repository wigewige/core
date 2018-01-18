using GenesisVision.Core.ViewModels.Investment;
using GenesisVision.Core.ViewModels.Manager;
using GenesisVision.DataModel.Models;
using System;
using System.Collections.Generic;

namespace GenesisVision.Core.Services.Validators.Interfaces
{
    public interface IManagerValidator
    {
        List<string> ValidateNewInvestmentRequest(ApplicationUser user, NewInvestmentRequest request);

        List<string> ValidateCreateManagerAccount(ApplicationUser user, NewManager request);

        List<string> ValidateCreateInvestmentProgram(ApplicationUser user, CreateInvestment investment);

        List<string> ValidateGetManagerDetails(ApplicationUser user, Guid managerId);

        List<string> ValidateUpdateManagerAccount(ApplicationUser user, UpdateManagerAccount account);

        List<string> ValidateCloseInvestmentProgram(ApplicationUser user, Guid investmentProgramId);
    }
}
