using System.Collections.Generic;
using GenesisVision.Core.Data.Models;
using GenesisVision.Core.ViewModels.Investment;
using GenesisVision.Core.ViewModels.Manager;
using System.Linq;

namespace GenesisVision.Core.Helpers
{
    public static class Convertors
    {
        public static Investment ToInvestment(this InvestmentPrograms inv)
        {
            return new Investment
                   {
                       Id = inv.Id,
                       InvestMinAmount = inv.InvestMinAmount,
                       InvestMaxAmount = inv.InvestMaxAmount,
                       Description = inv.Description,
                       IsEnabled = inv.IsEnabled,
                       FeeEntrance = inv.FeeEntrance,
                       FeeSuccess = inv.FeeSuccess,
                       FeeManagement = inv.FeeManagement,
                       DateFrom = inv.DateFrom,
                       DateTo = inv.DateTo,
                       Period = inv.Period,
                       ManagerId = inv.ManagersAccountId,
                       LastPeriod = inv.Periods?.OrderByDescending(x => x.Number).First().ToPeriod()
                   };
        }

        public static Period ToPeriod(this Periods p)
        {
            return new Period
                   {
                       Id = p.Id,
                       DateFrom = p.DateFrom,
                       DateTo = p.DateTo,
                       Status = p.Status,
                       Number = p.Number,
                       InvestmentRequest = p.InvestmentRequests?.Select(ToInvestmentRequest).ToList() ??
                                           new List<InvestmentRequest>()
                   };
        }

        public static ManagerRequest ToManagerRequest(this ManagerAccountRequests x)
        {
            return new ManagerRequest
                   {
                       Currency = x.Currency,
                       Description = x.Description,
                       Name = x.Name,
                       UserId = x.Id,
                       RequestId = x.Id
                   };
        }

        public static InvestmentRequest ToInvestmentRequest(this InvestmentRequests inv)
        {
            return new InvestmentRequest
                   {
                       Id = inv.Id,
                       Date = inv.Date,
                       Status = inv.Status,
                       Type = inv.Type,
                       Amount = inv.Amount
                   };
        }
    }
}
