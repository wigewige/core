using System;

namespace GenesisVision.DataModel.Models
{
    public class ManagersAccountsStatistics
    {
        public Guid Id { get; set; }
        public DateTime Date { get; set; }
        public decimal Fund { get; set; }
        public decimal Profit { get; set; }
        public decimal Loss { get; set; }
        public decimal Volume { get; set; }
        public decimal TotalProfit { get; set; }

        public ManagerAccounts ManagerAccount { get; set; }
        public Guid ManagerAccountId { get; set; }

        public Periods Period { get; set; }
        public Guid PeriodId { get; set; }
    }
}
