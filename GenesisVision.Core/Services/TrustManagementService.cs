using System;
using System.Collections.Generic;
using System.Linq;
using GenesisVision.Core.Data;
using GenesisVision.Core.Data.Models;
using GenesisVision.Core.Helpers;
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

        public OperationResult Invest(Invest model)
        {
            var invRequest = new InvestmentRequests
                             {
                                 Id = Guid.NewGuid(),
                                 UserId = model.UserId,
                                 Amount = model.Amount,
                                 Date = DateTime.Now,
                                 InvestmentProgramtId = model.InvestmentProgramId,
                                 Status = InvestmentRequestStatus.New,
                                 Type = model.RequestType
                             };
            context.Add(invRequest);
            context.SaveChanges();

            return OperationResult.Ok();
        }

        public OperationResult<List<Investment>> GetInvestments(InvestmentsFilter filter)
        {
            var query = context.InvestmentPrograms
                                .AsQueryable();

            if (filter.ManagerId.HasValue)
                query = query.Where(x => x.ManagersAccountId == filter.ManagerId);
            if (filter.BrokerId.HasValue)
                query = query.Where(x => x.ManagersAccount.BrokerTradeServer.BrokerId == filter.BrokerId);
            if (filter.BrokerTradeServerId.HasValue)
                query = query.Where(x => x.ManagersAccount.BrokerTradeServerId == filter.BrokerTradeServerId);
            if (filter.InvestMaxAmountFrom.HasValue)
                query = query.Where(x => x.InvestMinAmount >= filter.InvestMaxAmountFrom);
            if (filter.InvestMaxAmountTo.HasValue)
                query = query.Where(x => x.InvestMaxAmount < filter.InvestMaxAmountTo);

            if (filter.Skip.HasValue)
                query = query.Skip(filter.Skip.Value);
            if (filter.Take.HasValue)
                query = query.Take(filter.Take.Value);

            var result = query.Select(x => x.ToInvestment()).ToList();
            return OperationResult<List<Investment>>.Ok(result);
        }
    }
}
