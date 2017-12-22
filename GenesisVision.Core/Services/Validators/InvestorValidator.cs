using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using GenesisVision.Core.Data;
using GenesisVision.Core.Services.Validators.Interfaces;
using GenesisVision.Core.ViewModels.Investment;

namespace GenesisVision.Core.Services.Validators
{
    public class InvestorValidator : IInvestorValidator
    {
        private readonly ApplicationDbContext context;

        public InvestorValidator(ApplicationDbContext context)
        {
            this.context = context;
        }

        public List<string> ValidateInvest(IPrincipal user, Invest model)
        {
            var result = new List<string>();

            var investmentProgram = context.InvestmentPrograms.FirstOrDefault(x => x.Id == model.InvestmentProgramId);
            if (investmentProgram == null)
                return new List<string> {$"Does not find investment program id \"{model.InvestmentProgramId}\""};

            if (model.Amount <= 0)
                result.Add("Amount must be greater than zero");

            return result;
        }
    }
}
