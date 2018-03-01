using GenesisVision.Common.Models;
using GenesisVision.DataModel.Enums;
using System.Threading.Tasks;

namespace GenesisVision.Common.Services.Interfaces
{
    public interface IRateService
    {
        Task<decimal> GetRateAsync(Currency from, Currency to);

        OperationResult<decimal> GetRate(Currency from, Currency to);
    }
}
