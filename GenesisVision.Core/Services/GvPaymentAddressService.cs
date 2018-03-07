using GenesisVision.Common.Models;
using GenesisVision.Common.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace GenesisVision.Core.Services
{
    public class GvPaymentAddressService 
    {
		private readonly GvPaymentGatewayConfig config;

		public GvPaymentAddressService(GvPaymentGatewayConfig config)
		{
			this.config = config;
		}

		public EthAccount GenerateAddress()
		{
			return null;
		}
	}
}
