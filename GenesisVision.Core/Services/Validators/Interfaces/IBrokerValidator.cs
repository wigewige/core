using System;
using System.Collections.Generic;
using System.Security.Principal;

namespace GenesisVision.Core.Services.Validators.Interfaces
{
    public interface IBrokerValidator
    {
        List<string> ValidateGetBrokerInitData(IPrincipal user, Guid brokerTradeServerId);
    }
}
