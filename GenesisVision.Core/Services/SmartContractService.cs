using GenesisVision.Core.Models;
using GenesisVision.Core.Services.Interfaces;

namespace GenesisVision.Core.Services
{
    public class SmartContractService : ISmartContractService
    {
        public OperationResult RegisterManager(string tokenName, string tokenSymbol, string managerId, string managerLogin, string brokerId, decimal managementFee, decimal successFee)
        {
            return OperationResult.Ok();
        }

        public OperationResult SetInitialTokensHolder(string holderAddress)
        {
            return OperationResult.Ok();
        }
    }
}
