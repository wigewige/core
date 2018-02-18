using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GenesisVision.PaymentService.Models
{
    public class PaymentNotifyRequest
    {
        [Required]
        public string Type { get; set; }

        [Required]
        public string Tx_hash { get; set; }

        [Required]
        public string Address { get; set; }

        [Required]
        public string GatewayKey { get; set; }

        public decimal Amount { get; set; }

        [Range(0, int.MaxValue)]
        public int Confirmations { get; set; }
    }
}
