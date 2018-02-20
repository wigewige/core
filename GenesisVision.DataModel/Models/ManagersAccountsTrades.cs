using System;
using GenesisVision.DataModel.Enums;

namespace GenesisVision.DataModel.Models
{
    public class ManagersAccountsTrades
    {
        public Guid Id { get; set; }

        #region Common trade data

        public long Ticket { get; set; }
        public string Symbol { get; set; }
        public decimal Volume { get; set; }
        public decimal Profit { get; set; }
        public TradeDirectionType Direction { get; set; }

        #endregion

        #region MT4 trade data
        
        public DateTime? DateOpen { get; set; }
        public DateTime? DateClose { get; set; }
        public decimal? PriceOpen { get; set; }
        public decimal? PriceClose { get; set; }

        #endregion

        #region MT5 trade data

        public DateTime? Date { get; set; }
        public decimal? Price { get; set; }
        public TradeEntryType? Entry { get; set; }

        #endregion

        public ManagerAccounts ManagerAccount { get; set; }
        public Guid ManagerAccountId { get; set; }
    }
}
