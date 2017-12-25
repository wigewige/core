using System.Collections.Generic;
using GenesisVision.Core.ViewModels.Manager;

namespace GenesisVision.Core.ViewModels.Broker
{
    public class BrokerInitData
    {
        public List<ManagerRequest> NewManagerRequest { get; set; }
        public List<Investment.Investment> Investments { get; set; }
    }
}
