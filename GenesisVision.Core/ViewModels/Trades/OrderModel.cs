using GenesisVision.Core.ViewModels.Trades.Interfaces;
using GenesisVision.DataModel.Enums;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;

namespace GenesisVision.Core.ViewModels.Trades
{
    public class OrderModel : IBaseOrder, IMetaTrader4Order, IMetaTrader5Order
    {
        public Guid Id { get; set; }

        #region IBaseOrder

        public long Login { get; set; }
        public long Ticket { get; set; }
        public string Symbol { get; set; }
        public decimal Volume { get; set; }
        public decimal Profit { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public TradeDirectionType Direction { get; set; }

        #endregion

        #region IMetaTrader4Order

        public DateTime? DateOpen { get; set; }
        public DateTime? DateClose { get; set; }
        public decimal? PriceOpen { get; set; }
        public decimal? PriceClose { get; set; }

        #endregion

        #region IMetaTrader5Order

        public DateTime? Date { get; set; }
        public decimal? Price { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public TradeEntryType? Entry { get; set; }

        #endregion
    }
}
