using System;
using GenesisVision.DataModel.Enums;

namespace GenesisVision.DataModel.Models
{

    public class InvestmentRequests
    {
        public Guid Id { get; set; }
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
        public InvestmentRequestType Type { get; set; }
        public InvestmentRequestStatus Status { get; set; }

        public AspNetUsers User { get; set; }
        public Guid UserId { get; set; }

        public InvestmentPrograms InvestmentProgram { get; set; }
        public Guid InvestmentProgramtId { get; set; }

        public Periods Period { get; set; }
        public Guid PeriodId { get; set; }

        public InvestorAccounts InvestorAccount { get; set; }
        public Guid InvestorAccountId { get; set; }
    }
}
