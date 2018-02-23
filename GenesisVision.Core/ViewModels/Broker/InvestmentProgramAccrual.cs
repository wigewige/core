using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GenesisVision.Core.ViewModels.Broker
{
    public class InvestorAmount
    {
        public Guid InvestorId { get; set; }

        public decimal Amount { get; set; }
    }

    public class InvestmentProgramAccrual
    {
        public Guid InvestmentProgramId { get; set; }

        public List<InvestorAmount> Accruals { get; set; }
    }
}
