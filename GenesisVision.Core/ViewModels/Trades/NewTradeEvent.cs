using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GenesisVision.Core.ViewModels.Trades
{
    public class NewTradeEvent
    {
        [Required]
        public Guid ManagerAccountId { get; set; }

        [Required]
        public IEnumerable<OrderModel> Trades { get; set; }

        [Required]
        public decimal Balance { get; set; }
    }
}
