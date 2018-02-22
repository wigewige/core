using System;
using GenesisVision.DataModel.Enums;

namespace GenesisVision.DataModel.Models
{
    public class ManagersAccountsOpenTrades
    {
        public Guid Id { get; set; }
        public long Ticket { get; set; }
        public string Symbol { get; set; }
        public decimal Volume { get; set; }
        public decimal Price { get; set; }
        public decimal Profit { get; set; }
        public TradeDirectionType Direction { get; set; }
        public DateTime DateOpenOrder { get; set; }
        public DateTime DateUpdateFromTradePlatform { get; set; }

        public ManagerAccounts ManagerAccount { get; set; }
        public Guid ManagerAccountId { get; set; }
    }
}
