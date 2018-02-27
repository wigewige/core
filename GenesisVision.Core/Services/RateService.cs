using GenesisVision.Core.Services.Interfaces;
using GenesisVision.DataModel.Enums;

namespace GenesisVision.Core.Services
{
    public class RateService : IRateService
    {
        public decimal GetRate(Currency from, Currency to)
        {
            return 1;
        }
    }
}
