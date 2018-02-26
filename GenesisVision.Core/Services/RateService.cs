using GenesisVision.Core.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GenesisVision.Core.Services
{
    public class RateService : IRateService
    {
        public decimal GetRate(string from, string to)
        {
            return 1;
        }
    }
}
