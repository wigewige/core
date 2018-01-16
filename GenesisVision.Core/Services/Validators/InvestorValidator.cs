using GenesisVision.Core.Helpers;
using GenesisVision.Core.Services.Validators.Interfaces;
using GenesisVision.Core.ViewModels.Investment;
using GenesisVision.DataModel;
using GenesisVision.DataModel.Enums;
using GenesisVision.DataModel.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace GenesisVision.Core.Services.Validators
{
    public class InvestorValidator : IInvestorValidator
    {
        private readonly ApplicationDbContext context;

        public InvestorValidator(ApplicationDbContext context)
        {
            this.context = context;
        }

        public List<string> ValidateInvest(ApplicationUser user, Invest model)
        {
            if (!user.IsEnabled || user.Type != UserType.Investor)
                return new List<string> {ValidationMessages.AccessDenied};

            var result = new List<string>();

            // todo: validate investor wallet

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

        public List<string> ValidateWithdraw(ApplicationUser user, Invest model)
        {
            if (!user.IsEnabled || user.Type != UserType.Investor)
                return new List<string> {ValidationMessages.AccessDenied};

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
