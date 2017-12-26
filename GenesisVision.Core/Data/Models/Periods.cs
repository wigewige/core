using System;

namespace GenesisVision.Core.Data.Models
{
    public enum PeriodStatus
    {
        Planned = 0,
        InProccess = 1,
        Closed = 2
    }

    public class Periods
    {
        public Guid Id { get; set; }
        public int Number { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
        public PeriodStatus Status { get; set; }

        public InvestmentPrograms InvestmentProgram { get; set; }
        public Guid InvestmentProgramId { get; set; }
    }
}
