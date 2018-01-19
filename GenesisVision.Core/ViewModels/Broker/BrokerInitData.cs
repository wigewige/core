using GenesisVision.Core.ViewModels.Investment;
using GenesisVision.Core.ViewModels.Manager;
using System.Collections.Generic;

namespace GenesisVision.Core.ViewModels.Broker
{
    public class BrokerInitData
    {
        public List<ManagerRequest> NewManagerRequest { get; set; }
        public List<InvestmentProgram> Investments { get; set; }
    }
}
