using GenesisVision.Core.Helpers;
using GenesisVision.Core.Services.Validators.Interfaces;
using GenesisVision.Core.ViewModels.Broker;
using GenesisVision.DataModel;
using GenesisVision.DataModel.Enums;
using GenesisVision.DataModel.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using GenesisVision.Core.ViewModels.Trades;

namespace GenesisVision.Core.Services.Validators
{
    public class BrokerValidator : IBrokerValidator
    {
        private readonly ApplicationDbContext context;

        public BrokerValidator(ApplicationDbContext context)
        {
            this.context = context;
        }

        public List<string> ValidateGetBrokerInitData(ApplicationUser user, Guid brokerTradeServerId)
        {
            if (!user.IsEnabled || user.Type != UserType.Broker)
                return new List<string> {ValidationMessages.AccessDenied};

            var tradeServer = context
                .BrokerTradeServers
                .Include(x => x.Broker)
                .FirstOrDefault(x => x.Id == brokerTradeServerId);

            if (tradeServer == null)
                return new List<string> {$"Trade server id \"{brokerTradeServerId}\" does not exist"};

            if (user.Id != tradeServer.Broker.UserId)
                return new List<string> {ValidationMessages.AccessDenied};

            if (!tradeServer.IsEnabled || !tradeServer.Broker.IsEnabled)
                return new List<string> {$"Access denied for trade server id \"{brokerTradeServerId}\""};

            return new List<string>();
        }

        public List<string> ValidateGetClosingPeriodData(ApplicationUser user, Guid investmentProgramId)
        {
            if (!user.IsEnabled || user.Type != UserType.Broker)
                return new List<string> {ValidationMessages.AccessDenied};

            var result = new List<string>();

            var investmentProgram = context.InvestmentPrograms
                                           .Include(x => x.Periods)
                                           .Include(x => x.ManagerAccount)
                                           .ThenInclude(x => x.BrokerTradeServer)
                                           .ThenInclude(x => x.Broker)
                                           .FirstOrDefault(x => x.Id == investmentProgramId);
            if (investmentProgram == null)
                return new List<string> {$"Does not find investment program id \"{investmentProgramId}\""};

            if (user.Id != investmentProgram.ManagerAccount.BrokerTradeServer.Broker.UserId)
                return new List<string> {ValidationMessages.AccessDenied};

            return result;
        }

        public List<string> ValidateClosePeriod(ApplicationUser user, Guid investmentProgramId)
        {
            var periodErrors = ValidateGetClosingPeriodData(user, investmentProgramId);
            if (periodErrors.Any())
                return periodErrors;

            var period = context.Periods
                                .FirstOrDefault(x => x.InvestmentProgramId == investmentProgramId &&
                                                     x.Status == PeriodStatus.InProccess &&
                                                     x.DateTo <= DateTime.UtcNow);
            if (period == null)
                return new List<string> {"Does not find period for closing"};

            return new List<string>();
        }

        public List<string> ValidateCreateManagerAccount(ApplicationUser user, NewManager request)
        {
            if (!user.IsEnabled || user.Type != UserType.Broker)
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

        public List<string> ValidateSetPeriodStartValues(ApplicationUser user, Guid investmentProgramId)
        {
            var result = new List<string>();

            var periodErrors = ValidateClosePeriod(user, investmentProgramId);
            if (periodErrors.Any())
                return periodErrors;

            return result;
        }

        public List<string> ValidateAccrueProfits(ApplicationUser user, InvestmentProgramAccrual accrual)
        {
            var periodErrors = ValidateClosePeriod(user, accrual.InvestmentProgramId);
            if (periodErrors.Any())
                return periodErrors;

            var result = new List<string>();

            var investorIds = accrual.Accruals.Select(x => x.InvestorId).ToList();

            foreach (var acc in accrual.Accruals)
            {
                var investorTokens = context.InvestorTokens
                                            .Include(p => p.ManagerToken)
                                            .ThenInclude(p => p.InvestmentProgram)
                                            .Where(p => p.InvestorAccountId == acc.InvestorId)
                                            .FirstOrDefault(p => p.ManagerToken.InvestmentProgram.Id == accrual.InvestmentProgramId);

                if (investorTokens == null)
                    result.Add($"Investor {acc.InvestorId} doesn't belong to investment program");
            }

            return result;
        }

        public List<string> ValidateProcessInvestmentRequests(ApplicationUser user, Guid investmentProgramId)
        {
            var result = new List<string>();

            var periodErrors = ValidateClosePeriod(user, investmentProgramId);
            if (periodErrors.Any())
                return periodErrors;

            return result;
        }

        public List<string> ValidateNewTrade(ApplicationUser user, NewTradeEvent tradeEvent)
        {
            var result = new List<string>();

            var mangerAccount = context.ManagersAccounts
                                       .FirstOrDefault(x => x.Id == tradeEvent.ManagerAccountId &&
                                                            x.BrokerTradeServer.Broker.UserId == user.Id);
            if (mangerAccount == null)
                result.Add("Manager account does not exist");

            return result;
        }

        public List<string> ValidateUpdateManagerHistoryIpfsHash(ApplicationUser user, ManagerHistoryIpfsHash data)
        {
            var result = new List<string>();

            var ids = data.ManagersHashes.Select(x => x.ManagerId).Distinct().ToList();
            var mangerAccountCount = context.ManagersAccounts.Count(x => ids.Contains(x.Id));
            if (mangerAccountCount != ids.Count)
                result.Add("Manager account does not exist");

            return result;
        }

        public List<string> ValidateNewOpenTrades(ApplicationUser user, NewOpenTradesEvent trades)
        {
            var result = new List<string>();

            if (!trades.OpenTrades.Any())
                return result;

            var managersIds = trades.OpenTrades.Select(x => x.ManagerAccountId).ToList();
            var mangerAccount = context.ManagersAccounts
                                       .Where(x => managersIds.Contains(x.Id) &&
                                                   x.BrokerTradeServer.Broker.UserId == user.Id)
                                       .ToList();
            if (mangerAccount.Count != managersIds.Count)
                result.Add("Managers accounts does not exists");

            return result;
        }

        public List<string> ValidateReevaluateManagerToken(ApplicationUser user, Guid investmentProgramId)
        {
            var result = new List<string>();

            var periodErrors = ValidateClosePeriod(user, investmentProgramId);
            if (periodErrors.Any())
                return periodErrors;

            return result;
        }

        public List<string> ValidateProcessClosingProgram(ApplicationUser user, Guid investmentProgramId)
        {
            var result = new List<string>();

            var periodErrors = ValidateClosePeriod(user, investmentProgramId);
            if (periodErrors.Any())
                return periodErrors;

            var investmentProgram = context.InvestmentPrograms.First(x => x.Id == investmentProgramId);
            if (investmentProgram.DateTo == null || investmentProgram.DateTo > DateTime.UtcNow)
                result.Add("Investment program is still running and can't be closed");

            return result;
        }
    }
}
