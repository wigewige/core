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

        List<string> ValidateGetManagerDetails(ApplicationUser user, Guid managerId);

        List<string> ValidateCloseInvestmentProgram(ApplicationUser user, Guid investmentProgramId);

        List<string> ValidateInvest(ApplicationUser user, Invest model);

        List<string> ValidateWithdraw(ApplicationUser user, Invest model);

        List<string> ValidateCancelInvestmentRequest(ApplicationUser user, Guid requestId);
    }
}
