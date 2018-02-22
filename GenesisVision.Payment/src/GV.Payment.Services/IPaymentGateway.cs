using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace GV.Payment.Services
{
	public interface IPaymentGateway
	{
		string GatewayCode { get; }
		bool HasCurrency(string currencyCode);
		Task<WalletInfo> CreateWallet(string currencyCode, string userId, string trackingKey);
	}
}
