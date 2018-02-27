using GenesisVision.DataModel.Enums;

namespace GenesisVision.Core.Services.Interfaces
{
    public interface IRateService
    {
        decimal GetRate(Currency from, Currency to);
    }
}
