using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GenesisVision.Core.Models;
using GenesisVision.Core.Services.Interfaces;

namespace GenesisVision.Core.Services
{
    public class SmartContractService : ISmartContractService
    {
        public OperationResult RegisterManager(string tokenName, string tokenSymbol, string managerId, string managerLogin, string brokerId, uint managementFee, uint successFee)
        {
            throw new NotImplementedException();
        }

        public OperationResult SetInitialTokensHolder(string holderAddress)
        {
            throw new NotImplementedException();
        }
    }
}
