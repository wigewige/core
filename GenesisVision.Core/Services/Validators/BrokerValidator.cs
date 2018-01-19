using GenesisVision.Core.Helpers;
using GenesisVision.Core.Services.Validators.Interfaces;
using GenesisVision.DataModel;
using GenesisVision.DataModel.Enums;
using GenesisVision.DataModel.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

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
                                                     x.DateTo <= DateTime.Now);
            if (period == null)
                return new List<string> {"Does not find period for closing"};

            return new List<string>();
        }

        public List<string> ValidateSetPeriodStartBalance(ApplicationUser user, Guid periodId, decimal balance)
        {
            if (!user.IsEnabled || user.Type != UserType.Broker)
                return new List<string> {ValidationMessages.AccessDenied};

            var period = context.Periods
                                .Include(x => x.InvestmentProgram)
                                .ThenInclude(x => x.ManagerAccount)
                                .ThenInclude(x => x.BrokerTradeServer)
                                .ThenInclude(x => x.Broker)
                                .Include(x => x.InvestmentRequests)
                                .FirstOrDefault(x => x.Id == periodId);
            if (period == null)
                return new List<string> {$"Does not find period id \"{periodId}\""};

            if (user.Id != period.InvestmentProgram.ManagerAccount.BrokerTradeServer.Broker.UserId)
                return new List<string> {ValidationMessages.AccessDenied};

            if (period.Status != PeriodStatus.InProccess)
                return new List<string> {$"Period has status {period.Status}"};

            var result = new List<string>();

            var investmentsAmount = period.InvestmentRequests
                                          .Where(x => x.Type == InvestmentRequestType.Invest)
                                          .Sum(x => x.Amount);
            if (balance < investmentsAmount)
                result.Add("Balance could not be less than total investments");
            
            return result;
        }
    }
}
