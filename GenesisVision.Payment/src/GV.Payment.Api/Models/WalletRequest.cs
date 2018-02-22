using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GV.Payment.Api.Models
{
    public class WalletRequest
    {
		public string Currency { get; set; }
		public int Confirmations { get; set; }
		public int FeeLevel { get; set; }
		public string PayoutAddress { get; set; }
		public string CallbackUrl { get; set; }
	}
}
