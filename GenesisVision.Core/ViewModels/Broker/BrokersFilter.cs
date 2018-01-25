using GenesisVision.Core.ViewModels.Common;
using GenesisVision.DataModel.Enums;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace GenesisVision.Core.ViewModels.Broker
{
    public class BrokersFilter : PagingFilter
    {
        public string BrokerName { get; set; }

        public string TradeServerName { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public BrokerTradeServerType? TradeServerType { get; set; }
    }
}
