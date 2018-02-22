using GenesisVision.DataModel.Enums;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;

namespace GenesisVision.Core.ViewModels.Trades
{
    public class OpenOrderModel
    {
        public Guid Id { get; set; }
        public long Ticket { get; set; }
        public string Symbol { get; set; }
        public decimal Volume { get; set; }
        public decimal Price { get; set; }
        public decimal Profit { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public TradeDirectionType Direction { get; set; }
        public DateTime Date { get; set; }
    }
}
