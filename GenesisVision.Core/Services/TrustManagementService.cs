using System;
using System.Linq;
using GenesisVision.Core.Data;
using GenesisVision.Core.Data.Models;
using GenesisVision.Core.Models;
using GenesisVision.Core.Services.Interfaces;
using GenesisVision.Core.ViewModels.Investment;
using GenesisVision.Core.ViewModels.Manager;

namespace GenesisVision.Core.Services
{
    public class TrustManagementService : ITrustManagementService
    {
        private readonly ApplicationDbContext context;

        public TrustManagementService(ApplicationDbContext context)
        {
            this.context = context;
        }

        public OperationResult<Guid> CreateInvestmentProgram(CreateInvestment investment)
        {
            var inv = new InvestmentPrograms
                      {
                          Id = Guid.NewGuid(),
                          DateFrom = investment.DateFrom ?? DateTime.Now,
                          DateTo = investment.DateTo,
                          Description = investment.Description,
                          FeeEntrance = investment.FeeEntrance,
                          FeeManagement = investment.FeeManagement,
                          FeeSuccess = investment.FeeSuccess,
                          InvestMaxAmount = investment.InvestMaxAmount,
                          InvestMinAmount = investment.InvestMinAmount,
                          IsEnabled = true,
                          ManagersAccountId = investment.ManagersAccountId,
                          Period = investment.Period
                      };
            context.Add(inv);
            context.SaveChanges();

            return OperationResult<Guid>.Ok(inv.Id);
        }
    }
}
