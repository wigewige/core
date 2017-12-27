using GenesisVision.Core.Data;
using GenesisVision.Core.Services.Validators.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;

namespace GenesisVision.Core.Services.Validators
{
    public class BrokerValidator : IBrokerValidator
    {
        private readonly ApplicationDbContext context;

        public BrokerValidator(ApplicationDbContext context)
        {
            this.context = context;
        }

        public List<string> ValidateGetBrokerInitData(IPrincipal user, Guid brokerTradeServerId)
        {
            var result = new List<string>();

            var tradeServer = context
                .BrokerTradeServers
                .Include(x => x.Broker)
                .FirstOrDefault(x => x.Id == brokerTradeServerId);

            if (tradeServer == null)
                return new List<string> {$"Trade server id \"{brokerTradeServerId}\" does not exist"};

            // todo: check trade server belongs broker
            //if (user.UserId != brokerTradeServerId)
            //    return new List<string> {$"Access denied"};

            if (!tradeServer.IsEnabled || !tradeServer.Broker.IsEnabled)
                return new List<string> {$"Access denied for trade server id \"{brokerTradeServerId}\""};

            return result;
        }

        public List<string> ValidateGetClosingPeriodData(IPrincipal user, Guid investmentProgramId)
        {
            var result = new List<string>();

            var investmentProgram = context.InvestmentPrograms
                                           .Include(x => x.Periods)
                                           .FirstOrDefault(x => x.Id == investmentProgramId);
            if (investmentProgram == null)
                return new List<string> {$"Does not find investment program id \"{investmentProgramId}\""};

            // todo: check investment program belongs broker

            return result;
        }
    }
}
