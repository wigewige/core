using System;

namespace GenesisVision.Core.Data.Models
{
    public enum InvestmentRequestType
    {
        Invest = 0,
        Withdrawal = 1
    }

    public enum InvestmentRequestStatus
    {
        New = 0,
        Executed = 1
    }

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
