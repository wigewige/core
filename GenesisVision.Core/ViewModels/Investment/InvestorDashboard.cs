using GenesisVision.DataModel.Enums;
using System.Collections.Generic;
using System.Linq;

namespace GenesisVision.Core.ViewModels.Investment
{
    public class InvestorDashboard
    {
        public List<InvestorProgram> Programs { get; set; }
    }

    public class InvestorProgram
    {
        public InvestmentProgramDetails InvestmentProgram { get; set; }
        public List<InvestmentRequest> Requests { get; set; }

        public decimal TotalIn
        {
            get { return Requests.Where(x => x.Type == InvestmentRequestType.Invest).Sum(x => x.Amount); }
        }

        public decimal TotalOut
        {
            get { return Requests.Where(x => x.Type == InvestmentRequestType.Withdrawal).Sum(x => x.Amount); }
        }
    }
}
