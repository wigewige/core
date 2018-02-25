using GenesisVision.DataModel.Enums;
using System;
using System.Collections.Generic;

namespace GenesisVision.DataModel.Models
{
    public class Periods
    {
        public Guid Id { get; set; }
        public int Number { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
        public PeriodStatus Status { get; set; }
        public decimal StartBalance { get; set; }
        public decimal ManagerStartBalance { get; set; }
        public decimal ManagerStartShare { get; set; }

        public InvestmentPrograms InvestmentProgram { get; set; }
        public Guid InvestmentProgramId { get; set; }

        public ICollection<InvestmentRequests> InvestmentRequests { get; set; }

        public ICollection<ManagersAccountsStatistics> ManagersAccountsStatistics { get; set; }

        public ICollection<ProfitDistributionTransactions> ProfitDistributionTransactions { get; set; }
    }
}
