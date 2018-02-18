using GenesisVision.Core.ViewModels.Investment;
using GenesisVision.DataModel.Models;
using System;
using System.Collections.Generic;

namespace GenesisVision.Core.Services.Validators.Interfaces
{
    public interface IInvestorValidator
    {
        List<string> ValidateInvest(ApplicationUser user, Invest model);

        List<string> ValidateWithdraw(ApplicationUser user, Invest model);

        List<string> ValidateCancelInvestmentRequest(ApplicationUser user, Guid requestId);
    }
}
