using System;

namespace GV.Payment.Services
{
	public class WalletInfo
	{
		public Guid? WalletId { get; set; }
		public string Address { get; set; }
		public string Currency { get; set; }
		public string VerificationCode { get; set; }
		public string GatewayKey { get; set; }
		public DateTime Timestamp { get; set; }
		public string GatewayInvoice { get; set; }
	}
}
