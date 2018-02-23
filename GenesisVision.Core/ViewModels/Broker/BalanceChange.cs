using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GenesisVision.Core.ViewModels.Broker
{
    public class BalanceChange
    {
        public decimal ManagerBalanceChange { get; set; }

        public decimal AccountBalanceChange { get; set; }
    }
}
