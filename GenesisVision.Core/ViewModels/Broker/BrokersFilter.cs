using GenesisVision.Core.ViewModels.Other;
using GenesisVision.DataModel.Enums;

namespace GenesisVision.Core.ViewModels.Broker
{
    public class BrokersFilter : PagingFilter
    {
        public string BrokerName { get; set; }

        public string TradeServerName { get; set; }

        public BrokerTradeServerType? TradeServerType { get; set; }
    }
}
