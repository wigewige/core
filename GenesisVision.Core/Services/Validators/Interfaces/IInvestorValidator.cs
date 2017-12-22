using System.Collections.Generic;
using System.Security.Claims;
using System.Security.Principal;
using GenesisVision.Core.ViewModels.Investment;

namespace GenesisVision.Core.Services.Validators.Interfaces
{
    public interface IInvestorValidator
    {
        List<string> ValidateInvest(IPrincipal user, Invest model);
    }
}
