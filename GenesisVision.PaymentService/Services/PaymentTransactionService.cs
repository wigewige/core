using GenesisVision.DataModel;
using GenesisVision.DataModel.Enums;
using GenesisVision.DataModel.Models;
using GenesisVision.PaymentService.Models;
using GenesisVision.PaymentService.Services.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace GenesisVision.PaymentService.Services
{
	public class PaymentTransactionService : IPaymentTransactionService
    {
        private readonly ILogger<IPaymentTransactionService> logger;
        private readonly ApplicationDbContext context;

        public PaymentTransactionService(ILogger<IPaymentTransactionService> logger, ApplicationDbContext context)
        {
            this.logger = logger;
            this.context = context;
        }

        public async Task<PaymentTransactionInfo> ProcessCallback(ProcessPaymentTransaction request)
        {
            var blockchainAddress = context.BlockchainAddresses.FirstOrDefault(addr => addr.Address == request.Address);
            if (blockchainAddress == null)
            {
                var msg = $"Eth address not found - {request.Address}";
                logger.LogError(msg);
                throw new ApplicationException(msg);
            }

            if (request.Currency != blockchainAddress.Currency)
            {
                var msg = $"Wallet and Transaction currency should be the same {blockchainAddress.Id}";
                logger.LogError(msg);
                throw new ApplicationException(msg);
            }

            PaymentTransactionInfo paymentTransactionInfo;

            using (var transaction = await context.Database.BeginTransactionAsync())
            {
                var paymentTransaction = context.PaymentTransactions
                                                .FirstOrDefault(t => t.Hash == request.TransactionHash &&
                                                                     t.BlockchainAddressId == blockchainAddress.Id);

                if (paymentTransaction != null)
                {
                    if (paymentTransaction.Status == PaymentTransactionStatus.Pending ||
                        paymentTransaction.Status == PaymentTransactionStatus.New)
                    {
                        paymentTransaction.Status = request.Status;
                        paymentTransaction.LastUpdated = DateTime.UtcNow;
                        context.Update(paymentTransaction);
                        await context.SaveChangesAsync();
                    }
                    else
                    {
                        logger.LogError($"Can not override status from {paymentTransaction.Status} to {request.Status}");
                    }
                }
                else
                {
                    paymentTransaction = new PaymentTransactions
                                         {
                                             Id = Guid.NewGuid(),
                                             Hash = request.TransactionHash,
                                             Type = PaymentTransactionType.Deposit,
                                             BlockchainAddressId = blockchainAddress.Id,
                                             Amount = request.Amount,
                                             Fee = request.Fee,
                                             DateCreated = DateTime.UtcNow,
                                             Status = request.Status,
                                             PayoutTxHash = request.PayoutTxHash,
                                             PayoutServiceFee = request.PayoutServiceFee,
                                             PayoutMinerFee = request.PayoutMinerFee
                                         };

                    context.PaymentTransactions.Add(paymentTransaction);

                    if (paymentTransaction.Status == PaymentTransactionStatus.ConfirmedAndValidated)
                    {
                        var wallet = context.Wallets.First(w => w.UserId == blockchainAddress.UserId &&
                                                                w.Currency == blockchainAddress.Currency);
                        wallet.Amount += paymentTransaction.Amount;
                        await context.SaveChangesAsync();
                    }
                }

                transaction.Commit();

                paymentTransactionInfo = new PaymentTransactionInfo
                                         {
                                             TransactionId = paymentTransaction.Id,
                                             TransactionHash = paymentTransaction.Hash,
                                             Amount = paymentTransaction.Amount,
                                             Currency = request.Currency.ToString(),
                                             GatewayCode = request.GatewayCode,
                                             Status = paymentTransaction.Status,
                                             IsValid = true
                                         };
            }

            return paymentTransactionInfo;
        }
    }
}
