using System;
using System.Collections.Generic;

namespace GV.Payment.DataModel
{
	public partial class PaymentTransaction
	{
		public PaymentTransaction()
		{
		}

		public Guid Id { get; set; }
		public PaymentTransactionType Type { get; set; }		
		public string Hash { get; set; }
		public string GatewayCode { get; set; }
		public Guid WalletId { get; set; }
		public string Currency { get; set; }
		public decimal Amount { get; set; }
		public decimal Fee { get; set; }
		public PaymentTransactionStatus Status { get; set; }
		public DateTime DateCreated { get; set; }
		public DateTime? LastUpdated { get; set; }
		public DateTime? PaymentTxDate { get; set; }
		public string ExtraData { get; set; }
		public byte[] Timestamp { get; set; }
		public string PayoutTxHash { get; set; }
		public decimal? PayoutMinerFee { get; set; }
		public decimal? PayoutServiceFee { get; set; }
		public PaymentTransactionStatus PayoutStatus { get; set; }

		public virtual Wallet Wallet { get; set; }
		public int? LastUpdatedBlockNo { get; set; }
		public int? ConfirmationsCount { get; set; }
	}
}
