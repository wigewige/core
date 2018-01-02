using GenesisVision.PaymentService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GenesisVision.PaymentService.Services.Interfaces
{
    public interface IEthService
    {
        EthAccount GenerateAddress();
    }
}
