using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using GenesisVision.Core.Data;
using GenesisVision.Core.Data.Models;
using GenesisVision.Core.ViewModels.Investment;
using GenesisVision.Core.ViewModels.Manager;

namespace GenesisVision.Core.Services.Validators
{
    public class ManagerValidator : IManagerValidator
    {
        private readonly ApplicationDbContext context;

        public ManagerValidator(ApplicationDbContext context)
        {
            this.context = context;
        }

        public List<string> ValidateNewManagerAccountRequest(IPrincipal user, NewManagerRequest request)
        {
            var result = new List<string>();

            var server = context.BrokerTradeServers.FirstOrDefault(x => x.Id == request.BrokerTradeServerId);
            if (server == null)
                result.Add("Does not find trade server");

            if (request.UserId.HasValue)
            {
                var aspNetUser = context.AspNetUsers.FirstOrDefault(x => x.Id == request.UserId.Value);
                if (aspNetUser == null)
                {
                    result.Add("Does not find user");
                    return result;
                }
            }

            if (!string.IsNullOrEmpty(request.Name))
                result.Add("'Name' is empty");

            return result;
        }

        public List<string> ValidateCreateManagerAccount(IPrincipal user, NewManager request)
        {
            var result = new List<string>();

            // todo: check request belongs broker

            var req = context.ManagerRequests.FirstOrDefault(x => x.Id == request.RequestId);
            if (req == null)
            {
                result.Add("Does not find request");
                return result;
            }
            if (req.Status != ManagerRequestStatus.Created)
                result.Add($"Request status is {req.Status}");

            return result;
        }

        public List<string> ValidateCreateInvestmentProgram(IPrincipal user, CreateInvestment investment)
        {
            throw new System.NotImplementedException();
        }
    }
}
