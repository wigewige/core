using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace GenesisVision.Core.ViewModels.Trades
{
    public enum Direction
    {
        Buy = 0,
        Sell = 1
    }

    public class MetaTraderOrder : BaseTrade
    {
        public long Login { get; set; }
        public long Ticket { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public Direction Direction { get; set; }
    }
}
