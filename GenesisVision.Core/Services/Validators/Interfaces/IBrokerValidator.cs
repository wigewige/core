﻿using GenesisVision.DataModel.Models;
using System;
using System.Collections.Generic;

namespace GenesisVision.Core.Services.Validators.Interfaces
{
    public interface IBrokerValidator
    {
        List<string> ValidateGetBrokerInitData(ApplicationUser user, Guid brokerTradeServerId);

        List<string> ValidateGetClosingPeriodData(ApplicationUser user, Guid investmentProgramId);

        List<string> ValidateClosePeriod(ApplicationUser user, Guid investmentProgramId);

        List<string> ValidateSetPeriodStartBalance(ApplicationUser user, Guid periodId, decimal balance);
    }
}
