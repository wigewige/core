using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GenesisVision.Core.ViewModels.Trades
{
    public class NewOpenTradesEvent
    {
        [Required]
        public IEnumerable<ManagerOpenTrades> OpenTrades { get; set; }
    }

    public class ManagerOpenTrades
    {
        [Required]
        public Guid ManagerAccountId { get; set; }

        [Required]
        public IEnumerable<OpenOrderModel> Trades { get; set; }
    }
}
