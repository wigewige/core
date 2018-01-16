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

        public List<string> ValidateNewManagerAccountRequest(ApplicationUser user, NewManagerRequest request)
        {
            if (!user.IsEnabled || user.Type != UserType.Manager)
                return new List<string> {ValidationMessages.AccessDenied};

            var result = new List<string>();

            var server = context.BrokerTradeServers.FirstOrDefault(x => x.Id == request.BrokerTradeServerId);
            if (server == null)
                result.Add($"Does not find trade server id \"{request.BrokerTradeServerId}\"");

            if (request.UserId.HasValue)
            {
                var aspNetUser = context.Users.FirstOrDefault(x => x.Id == request.UserId.Value);
                if (aspNetUser == null)
                {
                    result.Add("Does not find user");
                    return result;
                }
            }

            if (string.IsNullOrEmpty(request.Name))
                result.Add("'Name' is empty");

            return result;
        }

        public List<string> ValidateCreateManagerAccount(ApplicationUser user, NewManager request)
        {
            if (!user.IsEnabled || user.Type != UserType.Manager)
                return new List<string> {ValidationMessages.AccessDenied};

            var result = new List<string>();

            var broker = context.BrokersAccounts
                                .Include(x => x.BrokerTradeServers)
                                .FirstOrDefault(x => x.UserId == user.Id);
            if (broker == null)
                return new List<string> {ValidationMessages.AccessDenied};
            
            var req = context.ManagerRequests.FirstOrDefault(x => x.Id == request.RequestId);
            if (req == null)
                return new List<string> {$"Does not find request with id \"{request.RequestId}\""};

            if (broker.BrokerTradeServers.All(x => x.Id != req.BrokerTradeServerId))
                return new List<string> {ValidationMessages.AccessDenied};

            if (req.Status != ManagerRequestStatus.Created)
                result.Add($"Could not proccess request. Request status is {req.Status}.");

            return result;
        }

        public List<string> ValidateCreateInvestmentProgram(ApplicationUser user, CreateInvestment investment)
        {
            if (!user.IsEnabled || user.Type != UserType.Manager)
                return new List<string> {ValidationMessages.AccessDenied};

            var result = new List<string>();
            
            var managerAccount = context.ManagersAccounts.FirstOrDefault(x => x.Id == investment.ManagersAccountId);
            if (managerAccount == null)
                return new List<string> {$"Does not find manager account \"{investment.ManagersAccountId}\""};

            if (managerAccount.UserId != user.Id)
                return new List<string> {ValidationMessages.AccessDenied};

            var existInvestmentsPrograms = context.InvestmentPrograms
                                                  .Where(x => x.ManagersAccountId == investment.ManagersAccountId &&
                                                              x.IsEnabled &&
                                                              (x.DateFrom <= DateTime.Now && (x.DateTo == null || x.DateTo > DateTime.Now)))
                                                  .ToList();
            if (existInvestmentsPrograms.Any())
                return new List<string> {"Manager has active investment program"};

            if (investment.DateFrom.HasValue && investment.DateFrom.Value.Date <= DateTime.Now.Date)
                result.Add("DateFrom must be greater than today");

            if (investment.DateFrom.HasValue && investment.DateTo.HasValue)
            {
                if (investment.DateFrom.Value >= investment.DateTo.Value)
                    result.Add("DateTo must be greater DateFrom");
                else if ((investment.DateTo.Value.Date - investment.DateFrom.Value.Date).TotalDays < 1)
                    result.Add("Minimum duration is 1 day");

                if (investment.Period > 0 && investment.DateFrom.Value.Date.AddDays(investment.Period) > investment.DateTo.Value.Date)
                    result.Add("DateTo must be greater first period");
            }
            else if (investment.DateTo.HasValue && investment.DateTo.Value.Date <= DateTime.Now.Date.AddDays(1))
                result.Add("DateTo must be greater than today");
            
            if (investment.FeeEntrance < 0)
                result.Add("FeeEntrance must be greater or equal zero");

            if (investment.FeeSuccess < 0)
                result.Add("FeeSuccess must be greater or equal zero");

            if (investment.FeeManagement < 0)
                result.Add("FeeManagement must be greater or equal zero");

            if (string.IsNullOrEmpty(investment.Description))
                result.Add("'Description' is empty");

            if (investment.Period <= 0)
                result.Add("Period must be greater than zero");

            return result;
        }

        public List<string> ValidateGetManagerDetails(ApplicationUser user, Guid managerId)
        {
            return context.ManagersAccounts.Any(x => x.Id == managerId)
                ? new List<string>()
                : new List<string> {$"Does not find manager account with id \"{managerId}\""};
        }

        public List<string> ValidateUpdateManagerAccount(ApplicationUser user, UpdateManagerAccount account)
        {
            var managerExistsErrors = ValidateGetManagerDetails(user, account.ManagerAccountId);
            if (managerExistsErrors.Any())
                return managerExistsErrors;

            if (!user.IsEnabled)
                return new List<string> {ValidationMessages.AccessDenied};

            var managerAccount = context.ManagersAccounts.First(x => x.Id == account.ManagerAccountId);
            if (managerAccount.UserId != user.Id)
                return new List<string> {ValidationMessages.AccessDenied};

            var result = new List<string>();

            if (string.IsNullOrEmpty(account.Description))
                result.Add("'Description' is empty");

            if (string.IsNullOrEmpty(account.Name))
                result.Add("'Name' is empty");

            return result;
        }
    }
}
