using System;
using System.Collections.Generic;

namespace GV.Payment.DataModel
{
    public partial class Wallet
    {
        public Wallet()
        {
            PaymentTransaction = new HashSet<PaymentTransaction>();
        }

        public Guid Id { get; set; }
        public string Address { get; set; }
        public string GatewayCode { get; set; }
        public string Currency { get; set; }
        public string PayoutAddress { get; set; }
        public string UserId { get; set; }
        public DateTime DateCreated { get; set; }
        public string GatewayKey { get; set; }
        public string GatewayInvoice { get; set; }
        public string GatewayData { get; set; }
        public string CustomKey { get; set; }
        public virtual ICollection<PaymentTransaction> PaymentTransaction { get; set; }
    }
}
