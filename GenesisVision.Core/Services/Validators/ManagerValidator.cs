using GenesisVision.Core.Helpers;
using GenesisVision.Core.Services.Validators.Interfaces;
using GenesisVision.Core.ViewModels.Investment;
using GenesisVision.Core.ViewModels.Manager;
using GenesisVision.DataModel;
using GenesisVision.DataModel.Enums;
using GenesisVision.DataModel.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GenesisVision.Core.Services.Validators
{
    public class ManagerValidator : IManagerValidator
    {
        private readonly ApplicationDbContext context;

        public ManagerValidator(ApplicationDbContext context)
        {
            this.context = context;
        }

        public List<string> ValidateNewInvestmentRequest(ApplicationUser user, NewInvestmentRequest request)
        {
            if (!user.IsEnabled || user.Type != UserType.Manager || user.Id != request.UserId)
                return new List<string> {ValidationMessages.AccessDenied};

            var result = new List<string>();

            var server = context.BrokerTradeServers.FirstOrDefault(x => x.Id == request.BrokerTradeServerId);
            if (server == null)
                result.Add($"Does not find trade server id \"{request.BrokerTradeServerId}\"");
            
            if (context.ManagerTokens.Any(x => x.TokenName == request.TokenName))
                result.Add("Token name is already exist");

            if (context.ManagerTokens.Any(x => x.TokenSymbol == request.TokenSymbol))
                result.Add("Token symbol is already exist");

            var wallet = context.Wallets.FirstOrDefault(x => x.UserId == user.Id && x.Currency == Currency.GVT);
            if (wallet == null || wallet.Amount < request.DepositAmount)
                result.Add(ValidationMessages.NotEnoughMoney);

            if (context.ManagerRequests.Any(x => x.Title == request.Title))
                result.Add("Title already exists");

            if (request.DateFrom.HasValue && request.DateFrom.Value.Date <= DateTime.Now.Date)
                result.Add("DateFrom must be greater than today");

            if (request.DateFrom.HasValue && request.DateTo.HasValue)
            {
                if (request.DateFrom.Value >= request.DateTo.Value)
                    result.Add("DateTo must be greater DateFrom");
                else if ((request.DateTo.Value.Date - request.DateFrom.Value.Date).TotalDays < 1)
                    result.Add("Minimum duration is 1 day");

                if (request.Period > 0 && request.DateFrom.Value.Date.AddDays(request.Period) > request.DateTo.Value.Date)
                    result.Add("DateTo must be greater first period");
            }
            else if (request.DateTo.HasValue && request.DateTo.Value.Date <= DateTime.Now.Date.AddDays(1))
                result.Add("DateTo must be greater than today");

            if (request.FeeEntrance < 0)
                result.Add("FeeEntrance must be greater or equal zero");

            if (request.FeeSuccess < 0)
                result.Add("FeeSuccess must be greater or equal zero");

            if (request.FeeManagement < 0)
                result.Add("FeeManagement must be greater or equal zero");

            if (string.IsNullOrEmpty(request.Description))
                result.Add("'Description' is empty");

            if (request.Period <= 0)
                result.Add("Period must be greater than zero");

            return result;
        }
        
        public List<string> ValidateCloseInvestmentProgram(ApplicationUser user, Guid investmentProgramId)
        {
            if (!user.IsEnabled || user.Type != UserType.Manager)
                return new List<string> {ValidationMessages.AccessDenied};

            var result = new List<string>();

            var investment = context.InvestmentPrograms
                                    .Include(x => x.ManagerAccount)
                                    .FirstOrDefault(x => x.Id == investmentProgramId);
            if (investment == null)
                return new List<string> {$"Does not find investment \"{investmentProgramId}\""};

            if (investment.ManagerAccount.UserId != user.Id)
                return new List<string> {ValidationMessages.AccessDenied};

            if (investment.DateTo.HasValue && investment.DateTo < DateTime.Now)
                result.Add("Investment already closed");

            return result;
        }

        public List<string> ValidateGetManagerDetails(ApplicationUser user, Guid managerId)
        {
            return context.ManagersAccounts.Any(x => x.Id == managerId)
                ? new List<string>()
                : new List<string> {$"Does not find manager account with id \"{managerId}\""};
        }

        public List<string> ValidateInvest(ApplicationUser user, Invest model)
        {
            if (!user.IsEnabled || user.Type != UserType.Manager || user.Id != model.UserId)
                return new List<string> { ValidationMessages.AccessDenied };

            var result = new List<string>();

            var wallet = context.Wallets.First(x => x.UserId == model.UserId && x.Currency == Currency.GVT);
            if (wallet.Amount < model.Amount)
                return new List<string> { ValidationMessages.NotEnoughMoney };

            var investmentProgram = context.InvestmentPrograms
                                           .Include(x => x.Periods)
                                           .FirstOrDefault(x => x.Id == model.InvestmentProgramId);
            if (investmentProgram == null)
                return new List<string> { $"Does not find investment program id \"{model.InvestmentProgramId}\"" };

            if (investmentProgram.ManagerAccountId != user.Id)
                return new List<string> { $"Manager can invest only in his own programs" };

            var lastPeriod = investmentProgram.Periods
                                              .OrderByDescending(x => x.Number)
                                              .FirstOrDefault();
            if (lastPeriod == null || lastPeriod.Status != PeriodStatus.Planned)
                return new List<string> { "There are no new period for investment program" };

            if (lastPeriod.InvestmentRequests.Any(x => x.UserId == user.Id && x.Type == InvestmentRequestType.Invest))
                return new List<string> { "Investment request can't be created having pending withdrawal request" };

            if (model.Amount <= 0)
                result.Add("Amount must be greater than zero");

            return result;
        }

        public List<string> ValidateWithdraw(ApplicationUser user, Invest model)
        {
            if (!user.IsEnabled || user.Type != UserType.Manager || user.Id != model.UserId)
                return new List<string> { ValidationMessages.AccessDenied };

            var result = new List<string>();

            var investmentProgram = context.InvestmentPrograms
                                           .Include(x => x.Periods)
                                           .ThenInclude(x => x.InvestmentRequests)
                                           .FirstOrDefault(x => x.Id == model.InvestmentProgramId);
            if (investmentProgram == null)
                return new List<string> { $"Does not find investment program id \"{model.InvestmentProgramId}\"" };

            if (investmentProgram.ManagerAccountId != user.Id)
                return new List<string> { $"Manager can withdraw only from his own programs" };

            var lastPeriod = investmentProgram.Periods
                                              .OrderByDescending(x => x.Number)
                                              .FirstOrDefault();
            if (lastPeriod == null || lastPeriod.Status != PeriodStatus.Planned)
                return new List<string> { "There are no new period for investment program" };

            if (lastPeriod.InvestmentRequests.Any(x => x.UserId == user.Id && x.Type == InvestmentRequestType.Invest))
                return new List<string> { "Withdrawal request can't be created having pending investment request" };

            if (model.Amount <= 0)
                result.Add("Amount must be greater than zero");

            return result;
        }

        public List<string> ValidateCancelInvestmentRequest(ApplicationUser user, Guid requestId)
        {
            if (!user.IsEnabled || user.Type != UserType.Manager)
                return new List<string> { ValidationMessages.AccessDenied };

            var result = new List<string>();

            var investmentRequest = context.InvestmentRequests
                                    .FirstOrDefault(x => x.Id == requestId
                                    && x.Status == InvestmentRequestStatus.New
                                    && x.UserId == user.Id);

            if (investmentRequest == null)
                return new List<string> { "No investment request found" };

            return result;
        }
    }
}
