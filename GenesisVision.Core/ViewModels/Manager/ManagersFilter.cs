using GenesisVision.DataModel.Models;
using GenesisVision.Core.ViewModels.Other;
using GenesisVision.DataModel.Enums;

namespace GenesisVision.Core.ViewModels.Manager
{
    public class ManagersFilter : Paging
    {
        public string Name { get; set; }

        public string BrokerName { get; set; }
        
        public string BrokerTradeServerName { get; set; }
        public BrokerTradeServerType? BrokerTradeServerType { get; set; }
    }
}
