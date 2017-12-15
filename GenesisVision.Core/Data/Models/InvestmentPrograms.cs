using System;
using System.Collections.Generic;

namespace GenesisVision.Core.Data.Models
{
    public class InvestmentPrograms
    {
        public Guid Id { get; set; }
        public string Description { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public bool IsEnabled { get; set; }
        public int Period { get; set; }
        public decimal FeeSuccess { get; set; }
        public decimal FeeManagement { get; set; }
        public decimal FeeEntrance { get; set; }
        public decimal InvestMinAmount { get; set; }
        public decimal? InvestMaxAmount { get; set; }

        public ManagerAccounts ManagersAccount { get; set; }
        public Guid ManagersAccountId { get; set; }

        public ICollection<InvestmentRequests> InvestmentRequests { get; set; }
    }
}
