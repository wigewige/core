using GenesisVision.Common.Models;

namespace GenesisVision.Common.Services.Interfaces
{
    public interface IEthService
    {
        EthAccount GenerateAddress();
    }
}
