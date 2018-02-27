using GenesisVision.Core.ViewModels.Broker;
using GenesisVision.Core.ViewModels.Trades;
using GenesisVision.DataModel.Models;
using System;
using System.Collections.Generic;

namespace GenesisVision.Core.Services.Validators.Interfaces
{
    public interface IBrokerValidator
    {
        List<string> ValidateGetBrokerInitData(ApplicationUser user, Guid brokerTradeServerId);

        List<string> ValidateGetClosingPeriodData(ApplicationUser user, Guid investmentProgramId);

        List<string> ValidateClosePeriod(ApplicationUser user, Guid investmentProgramId);

        List<string> ValidateCreateManagerAccount(ApplicationUser user, NewManager request);

        List<string> ValidateSetPeriodStartValues(ApplicationUser user, Guid periodId, decimal balance);

        List<string> ValidateAccrueProfits(ApplicationUser user, InvestmentProgramAccrual accrual);

        List<string> ValidateProcessInvestmentRequests(ApplicationUser user, Guid investmentProgramId);

        List<string> ValidateNewTrade(ApplicationUser user, NewTradeEvent tradeEvent);

        List<string> ValidateNewOpenTrades(ApplicationUser user, NewOpenTradesEvent trades);

        List<string> ValidateReevaluateManagerToken(ApplicationUser user, Guid investmentProgramId);
    }
}
