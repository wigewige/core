using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GV.Payment.Api.Models
{
    public class WalletResponse
    {
		public string Address { get; set; }
		public string WalletId { get; set; }
		public string VerificationCode { get; set; }
		public string Currency { get; internal set; }
	}
}
