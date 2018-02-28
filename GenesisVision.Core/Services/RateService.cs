using System;
using GenesisVision.Core.Models;
using GenesisVision.Core.Services.Interfaces;
using GenesisVision.DataModel.Enums;

namespace GenesisVision.Core.Services
{
    public class RateService : IRateService
    {
        public OperationResult<decimal> GetRate(Currency from, Currency to)
        {
            return InvokeOperations.InvokeOperation(() =>
            {
                if (from == Currency.Undefined || to == Currency.Undefined)
                    throw new Exception("Wrong currency");

                return 1m;
            });
        }
    }
}
