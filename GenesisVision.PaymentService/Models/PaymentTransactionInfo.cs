using GenesisVision.DataModel.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GenesisVision.PaymentService.Models
{
    public class PaymentTransactionInfo
    {
        public Guid TransactionId { get; set; }
        public string GetTransactionHash()
        {
            if (TransactionHash != null)
            {
                if (TransactionHash.Contains("#"))
                {
                    return TransactionHash.Split('#')[0];
                }
                return TransactionHash;
            }
            return null;
        }
        public string TransactionHash { get; set; }
        public string GatewayCode { get; set; }
        public string GatewayKey { get; set; }
        public PaymentTransactionStatus Status { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public bool IsValid { get; set; }
        public DateTime LastUpdated { get; set; }
        public DateTime DateCreated { get; set; }

    }
}
