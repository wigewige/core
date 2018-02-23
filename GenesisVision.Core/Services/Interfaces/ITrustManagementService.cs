using System;
using System.Collections.Generic;
using GenesisVision.Core.Models;
using GenesisVision.Core.ViewModels.Broker;
using GenesisVision.Core.ViewModels.Investment;
using GenesisVision.Core.ViewModels.Manager;

namespace GenesisVision.Core.Services.Interfaces
{
    public interface ITrustManagementService
    {
        OperationResult Invest(Invest model);

        OperationResult RequestForWithdraw(Invest model);

        OperationResult CloseInvestmentProgram(Guid invProgramId);

        OperationResult<Guid> CreateInvestmentProgram(NewManager request);

        OperationResult<(List<InvestmentProgram>, int)> GetInvestments(InvestmentsFilter filter);

        OperationResult<List<InvestmentProgram>> GetBrokerInvestmentsInitData(Guid brokerTradeServerId);

        OperationResult<ClosePeriodData> GetClosingPeriodData(Guid investmentProgramId);

        OperationResult ClosePeriod(Guid investmentProgramId);

        OperationResult SetPeriodStartBalance(Guid periodId, decimal balance);

        OperationResult<(List<BrokerTradeServer>, int)> GetBrokerTradeServers(BrokersFilter filter);

        OperationResult<InvestmentProgram> GetInvestment(Guid investmentId);

        OperationResult<InvestorDashboard> GetInvestorDashboard(Guid investorUserId);

        OperationResult AccrueProfits(InvestmentProgramAccrual accrual);

        OperationResult CancelInvestmentRequest(Guid requestId);

        OperationResult<BalanceChange> ProcessInvestmentRequests(Guid investmentProgramId);
    }
}
