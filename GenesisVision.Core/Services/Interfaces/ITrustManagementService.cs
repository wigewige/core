using GenesisVision.Core.Models;
using GenesisVision.Core.ViewModels.Broker;
using GenesisVision.Core.ViewModels.Investment;
using System;
using System.Collections.Generic;

namespace GenesisVision.Core.Services.Interfaces
{
    public interface ITrustManagementService
    {
        OperationResult Invest(Invest model);

        OperationResult RequestForWithdraw(Invest model);

        OperationResult CloseInvestmentProgram(Guid invProgramId);

        OperationResult<Guid> CreateInvestmentProgram(NewManager request);

        OperationResult<(List<InvestmentProgram>, int)> GetInvestmentPrograms(InvestmentProgramsFilter filter, Guid? userId);

        OperationResult<List<BrokerInvestmentProgram>> GetBrokerInvestmentsInitData(Guid brokerTradeServerId);

        OperationResult<ClosePeriodData> GetClosingPeriodData(Guid investmentProgramId);

        OperationResult ClosePeriod(Guid investmentProgramId);

        OperationResult SetPeriodStartValues(Guid investmentProgramId, decimal balance, decimal managerBalance, decimal managerShare);

        OperationResult<(List<BrokerTradeServer>, int)> GetBrokerTradeServers(BrokersFilter filter);

        OperationResult<InvestmentProgramDetails> GetInvestmentProgram(Guid investmentId, Guid? userId);

        OperationResult<InvestorDashboard> GetInvestorDashboard(Guid investorUserId, Guid? userId);

        OperationResult AccrueProfits(InvestmentProgramAccrual accrual);

        OperationResult CancelInvestmentRequest(Guid requestId);

        OperationResult<BalanceChange> ProcessInvestmentRequests(Guid investmentProgramId);

        OperationResult ReevaluateManagerToken(Guid investmentProgramId, decimal investorLossShare);
    }
}
