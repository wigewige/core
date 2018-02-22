using System.Collections.Generic;

namespace GenesisVision.Core.ViewModels.Trades
{
    public class OpenTradesViewModel
    {
        public List<OpenOrderModel> Trades { get; set; }

        public int Total { get; set; }
    }
}
