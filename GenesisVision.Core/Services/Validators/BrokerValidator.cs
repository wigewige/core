using GenesisVision.Core.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using GenesisVision.Core.Services.Validators.Interfaces;

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
    }
}
