using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GV.Payment.Api.Models
{
	public class WalletBalance
	{
		public string Address { get; set; }
		public string Currency { get; set; }
		public decimal Amount { get; set; }
		public string AmountData { get; set; }
	}
}
