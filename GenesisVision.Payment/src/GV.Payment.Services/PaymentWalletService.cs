using GV.Payment.DataModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace GV.Payment.Services
{
	public interface IPaymentWalletService
	{

	}

	public class PaymentWalletService : IPaymentWalletService
	{
		private readonly IEnumerable<IPaymentGateway> gateways;
		private readonly ILogger<IPaymentWalletService> logger;
		private readonly PaymentContext paymentDbContext;

		public PaymentWalletService(IEnumerable<IPaymentGateway> gateways, ILogger<IPaymentWalletService> logger, PaymentContext paymentDbContext)
		{
			this.gateways = gateways;
			this.logger = logger;
			this.paymentDbContext = paymentDbContext;
		}

		public async Task<WalletInfo> GenerateWallet(string currency, string userId, string payoutAddress)
		{
			//logger.LogInformation("Start {Method} {Currency} {WalletType} {UserId}", nameof(GenerateWallet), currency, icoStage, userId);

			//var gateway = GetGateway(currency);
			//if (gateway == null)
			//{
			//	logger.LogCritical($"gateway for currency {currency} was not found");
			//	throw new PaymentException($"gateway for currency {currency} was not found");
			//}

			WalletInfo result = null;
			var customKey = CreateKey(userId);
			try
			{
				result = await ReserveWalletAddress(currency, userId);
			}
			catch (Exception e)
			{
				logger.LogWarning("Wallet was not created {Method} {UserId} {Exception}", nameof(GenerateWallet), userId, e.ToString());

				logger.LogError("Wallet was not created {Method} {UserId} {Exception}", nameof(GenerateWallet), userId, e.ToString());
			}

			if (result == null)
			{
				logger.LogError("Wallet was not created {Method} {UserId}", nameof(GenerateWallet), userId);
				return null;//TODO
			}

			using (var transaction = await paymentDbContext.Database.BeginTransactionAsync())
			{
				var wallet = new Wallet()
				{
					DateCreated = DateTime.UtcNow,
					GatewayCode = "GV",
					Currency = currency,
					Address = result.Address,
					PayoutAddress = payoutAddress,
					UserId = userId,
					GatewayKey = result.GatewayKey,
					GatewayInvoice = result.GatewayInvoice,
					CustomKey = customKey
				};

				await paymentDbContext.Wallet.AddAsync(wallet);
				await paymentDbContext.SaveChangesAsync();
				transaction.Commit();

				result.WalletId = wallet.Id;
				logger.LogWarning("GenerateWalletOK Id:{Id};Currency:{Currency};Address:{Address};EthAddress:{EthAddress};UserId:{UserId}", wallet.Id, wallet.Currency, result.Address, wallet.PayoutAddress, wallet.UserId);

				logger.LogInformation("Wallet created {WalletId}", wallet.Id);
			}

			logger.LogInformation("End {Method} {Currency} {WalletType} {UserId}", nameof(GenerateWallet), currency, "", userId);

			return result;
		}

		public async Task<WalletInfo> ReserveWalletAddress(string currencyCode, string userId)
		{
			using (var transaction = await paymentDbContext.Database.BeginTransactionAsync())
			{
				var newWallet = await paymentDbContext
					.AddressStorage
					.Where(x => x.IsUsed == false && x.Currency == currencyCode)
					.FirstOrDefaultAsync();

				if (newWallet != null)
				{
					newWallet.IsUsed = true;
					newWallet.LastUpdated = DateTime.UtcNow;

					await paymentDbContext.SaveChangesAsync();
					transaction.Commit();

					return new WalletInfo()
					{
						Address = newWallet.Address,
						GatewayKey = "TODO" + "-" + newWallet.Address.ToLower(),
						Currency = currencyCode
					};
				}
			}
			throw new PaymentException($"Can't Generate Address.");
		}

		public static string CreateKey(string input)
		{
			const string salt = "sha256.ComputeHash(Encoding.UTF8.GetBytes(data";
			// Use input string to calculate MD5 hash
			using (MD5 md5 = MD5.Create())
			{
				byte[] inputBytes = Encoding.ASCII.GetBytes(input + salt);
				byte[] hashBytes = md5.ComputeHash(inputBytes);

				// Convert the byte array to hexadecimal string
				var sb = new StringBuilder();
				for (int i = 0; i < hashBytes.Length; i++)
				{
					sb.Append(hashBytes[i].ToString("X2"));
				}
				return sb.ToString();
			}
		}
	}

	public class PaymentException : Exception
	{
		public PaymentException() : base()
		{
		}

		public PaymentException(string message) : base(message)
		{

		}

		public PaymentException(string message, Exception innerException) : base(message, innerException)
		{
		}
	}
}
