using GenesisVision.DataModel.Enums;

namespace GenesisVision.PaymentService.Models
{
    public class ProcessPaymentTransaction
    {
        public string TransactionHash { get; set; }
        public string GatewayCode { get; set; }
        /// <summary>
        /// address or payment_code
        /// </summary>
        public decimal Amount { get; set; }
        public decimal Fee { get; set; }
        public Currency Currency { get; set; }

        public PaymentTransactionStatus Status { get; set; }
        public string CustomKey { get; set; }
        public string Address { get; set; }

        public string PayoutTxHash { get; set; }
        public decimal? PayoutMinerFee { get; set; }
        public decimal? PayoutServiceFee { get; set; }
        public PaymentTransactionStatus PayoutStatus { get; set; }
    }
}
