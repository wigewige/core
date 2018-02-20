using GenesisVision.DataModel.Enums;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;

namespace GenesisVision.Core.ViewModels.Trades
{
    public abstract class BaseOrder
    {
        public Guid Id { get; set; }
        public long Ticket { get; set; }
        public string Symbol { get; set; }
        public decimal Volume { get; set; }
        public decimal Profit { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public TradeDirectionType Direction { get; set; }
    }
}
