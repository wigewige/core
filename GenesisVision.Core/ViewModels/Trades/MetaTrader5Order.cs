using GenesisVision.DataModel.Enums;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;

namespace GenesisVision.Core.ViewModels.Trades
{
    public class MetaTrader5Order : BaseOrder
    {
        public DateTime Date { get; set; }
        public decimal Price { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public TradeEntryType Entry { get; set; }
    }
}
