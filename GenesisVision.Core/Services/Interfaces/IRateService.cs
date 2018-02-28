using GenesisVision.Core.Models;
using GenesisVision.DataModel.Enums;

namespace GenesisVision.Core.Services.Interfaces
{
    public interface IRateService
    {
        OperationResult<decimal> GetRate(Currency from, Currency to);
    }
}
