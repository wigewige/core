using System.Collections.Generic;
using System.Security.Principal;
using GenesisVision.Core.ViewModels.Investment;
using GenesisVision.Core.ViewModels.Manager;

namespace GenesisVision.Core.Services.Validators
{
    public interface IManagerValidator
    {
        List<string> ValidateNewManagerAccountRequest(IPrincipal user, NewManagerRequest request);

        List<string> ValidateCreateManagerAccount(IPrincipal user, NewManager request);

        List<string> ValidateCreateInvestmentProgram(IPrincipal user, CreateInvestment investment);
    }
}
