using System;
using System.Collections.Generic;

namespace GenesisVision.DataModel.Models
{
    public class InvestmentPrograms
    {
        public Guid Id { get; set; }
        public string Description { get; set; }
        public string Logo { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public bool IsEnabled { get; set; }
        public int Period { get; set; }
        public decimal FeeSuccess { get; set; }
        public decimal FeeManagement { get; set; }
        public decimal FeeEntrance { get; set; }
        public decimal InvestMinAmount { get; set; }
        public decimal? InvestMaxAmount { get; set; }
        public decimal Rating { get; set; }
        public int OrdersCount { get; set; }
        public decimal TotalProfit { get; set; }

        public ManagerAccounts ManagerAccount { get; set; }
        public Guid ManagerAccountId { get; set; }

        public ManagerTokens Token { get; set; }
        public Guid ManagerTokensId { get; set; }

        public ICollection<InvestmentRequests> InvestmentRequests { get; set; }

        public ICollection<Periods> Periods { get; set; }

        public ICollection<ManagersAccountsStatistics> ManagersAccountsStatistics { get; set; }
    }
}
