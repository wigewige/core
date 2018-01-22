using System;

namespace GenesisVision.DataModel.Models
{
    public class ManagersAccountsStatistics
    {
        public Guid Id { get; set; }
        public DateTime Date { get; set; }
        public decimal CurrentBalance { get; set; }
        public decimal Profit { get; set; }
        
        public ApplicationUser User { get; set; }
        public Guid UserId { get; set; }

        public ManagerAccounts ManagerAccount { get; set; }
        public Guid ManagerAccountId { get; set; }

        public InvestmentPrograms InvestmentProgram { get; set; }
        public Guid InvestmentProgramId { get; set; }

        public Periods Period { get; set; }
        public Guid PeriodId { get; set; }
    }
}
