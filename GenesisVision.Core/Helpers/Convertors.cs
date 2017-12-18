using GenesisVision.Core.Data.Models;
using GenesisVision.Core.ViewModels.Investment;
using GenesisVision.Core.ViewModels.Manager;

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
                       ManagerId = inv.ManagersAccountId
                   };
        }

        public static ManagerRequest ToManagerRequest(this ManagerAccountRequests x)
        {
            return new ManagerRequest
                   {
                       Currency = x.Currency,
                       Description = x.Description,
                       Name = x.Name
                   };
        }
    }
}
