using GenesisVision.DataModel;
using GenesisVision.DataModel.Models;
using GenesisVision.Core.Services.Validators.Interfaces;
using GenesisVision.Core.ViewModels.Investment;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;

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

            var investmentProgram = context.InvestmentPrograms
                                           .Include(x => x.Periods)
                                           .FirstOrDefault(x => x.Id == model.InvestmentProgramId);
            if (investmentProgram == null)
                return new List<string> {$"Does not find investment program id \"{model.InvestmentProgramId}\""};

            var lastPeriod = investmentProgram.Periods
                                              .OrderByDescending(x => x.Number)
                                              .FirstOrDefault();
            if (lastPeriod == null || lastPeriod.Status != PeriodStatus.Planned)
                return new List<string> {"There are no new period for investment program"};
            
            if (model.Amount <= 0)
                result.Add("Amount must be greater than zero");

            return result;
        }

        public List<string> ValidateWithdraw(IPrincipal user, Invest model)
        {
            var result = new List<string>();

            var investmentProgram = context.InvestmentPrograms
                                           .Include(x => x.Periods)
                                           .FirstOrDefault(x => x.Id == model.InvestmentProgramId);
            if (investmentProgram == null)
                return new List<string> {$"Does not find investment program id \"{model.InvestmentProgramId}\""};

            // todo: validations

            return result;
        }
    }
}
