using System;
using System.Collections;
using System.Collections.Generic;
using GenesisVision.DataModel.Enums;

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

        public InvestmentPrograms InvestmentProgram { get; set; }
        public Guid InvestmentProgramId { get; set; }

        public ICollection<InvestmentRequests> InvestmentRequests { get; set; }

        public ICollection<ManagersAccountsStatistics> ManagersAccountsStatistics { get; set; }
    }
}
