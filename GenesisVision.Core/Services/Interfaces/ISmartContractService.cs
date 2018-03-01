using GenesisVision.Common.Models;

namespace GenesisVision.Core.Services.Interfaces
{
    public interface ISmartContractService
    {
        OperationResult RegisterManager(string tokenName, string tokenSymbol, string managerId, string managerLogin, string brokerId, decimal managementFee, decimal successFee); // TODO uint8

        OperationResult SetInitialTokensHolder(string holderAddress);
    }
}
