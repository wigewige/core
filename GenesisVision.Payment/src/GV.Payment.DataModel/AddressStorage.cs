using System;
using System.Collections.Generic;
using System.Text;

namespace GV.Payment.DataModel
{
	public partial class AddressStorage
    {
		public long Id { get; set; }
		public int Type { get; set; }
		public string Address { get; set; }
		public string Currency { get; set; }
		public bool IsUsed { get; set; }
		public string GatewayKey { get; set; }
		public DateTime DateCreated { get; set; }
		public DateTime? LastUpdated { get; set; }
	}
}
