using GenesisVision.Core.Models;
using GenesisVision.Core.ViewModels.Manager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GenesisVision.Core.Services.Interfaces
{
    public interface ISmartContractService
    {
        OperationResult RegisterManager(string tokenName, string tokenSymbol, string managerId, string managerLogin, string brokerId, decimal managementFee, decimal successFee); // TODO uint8

        OperationResult SetInitialTokensHolder(string holderAddress);
    }
}
