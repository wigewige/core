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
        OperationResult<Guid> CreateInvestmentProgram(CreateInvestment investment);

        OperationResult Invest(Invest model);

        OperationResult CloseInvestmentProgram(Guid invProgramId);

        OperationResult<(List<Investment>, int)> GetInvestments(InvestmentsFilter filter);

        OperationResult<List<Investment>> GetBrokerInvestmentsInitData(Guid brokerTradeServerId);

        OperationResult<ClosePeriodData> GetClosingPeriodData(Guid investmentProgramId);

        OperationResult ClosePeriod(Guid investmentProgramId);

        OperationResult SetPeriodStartBalance(Guid periodId, decimal balance);

        OperationResult<(List<BrokerTradeServer>, int)> GetBrokerTradeServers(BrokersFilter filter);
    }
}
