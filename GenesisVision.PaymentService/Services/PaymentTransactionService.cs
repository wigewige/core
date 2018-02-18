using GenesisVision.DataModel;
using GenesisVision.DataModel.Enums;
using GenesisVision.DataModel.Models;
using GenesisVision.PaymentService.Services.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace GenesisVision.PaymentService.Models
{
    public class PaymentTransactionService : IPaymentTransactionService
    {
        private readonly ILogger<IPaymentTransactionService> logger;
        private readonly ApplicationDbContext db;

        public PaymentTransactionService(ILogger<IPaymentTransactionService> logger, ApplicationDbContext dbContext)
        {
            this.logger = logger;
            this.db = dbContext;
        }

        public async Task<PaymentTransactionInfo> ProcessCallback(ProcessPaymentTransaction request)
        {
            var wallet = db.EthAddresses.FirstOrDefault(addr => addr.Address == request.Address);

            if (wallet == null)
            {
                logger.LogError($"Eth address not found - {request.Address}");
                throw new ApplicationException($"Eth address not found - {request.Address}");
            }

            if (request.Currency != wallet.Currency)
            {
                logger.LogError("Wallet and Transaction currency should be the same {WalletId}", wallet.Id);
            }

            var paymentTransactionInfo = new PaymentTransactionInfo();

            using (var transaction = await db.Database.BeginTransactionAsync())
            {
                var paymentTransaction = db.PaymentTransactions
                    .FirstOrDefault(t => t.Hash == request.TransactionHash && t.WalletId == wallet.Id);

                if(paymentTransaction != null)
                {
                    if(paymentTransaction.Status == PaymentTransactionStatus.Pending 
                        || paymentTransaction.Status == PaymentTransactionStatus.New)
                    {
                        paymentTransaction.Status = request.Status;
                        paymentTransaction.LastUpdated = DateTime.UtcNow;
                        db.Update(paymentTransaction);
                        await db.SaveChangesAsync();
                    }
                    else
                    {
                        logger.LogError("Can not override status from {TransactionStatus} to {RequestStatus}", paymentTransaction.Status, request.Status);
                    }
                }
                else
                {
                    paymentTransaction = new PaymentTransactions
                    {
                        Id = Guid.NewGuid(),
                        Hash = request.TransactionHash,
                        Wallet = wallet,
                        WalletId = wallet.Id,
                        Amount = request.Amount,
                        Fee = request.Fee,
                        DateCreated = DateTime.UtcNow,
                        Status = request.Status,
                        PayoutTxHash = request.PayoutTxHash,
                        PayoutServiceFee = request.PayoutServiceFee,
                        PayoutMinerFee = request.PayoutMinerFee
                    };

                    db.PaymentTransactions.Add(paymentTransaction);
                    await db.SaveChangesAsync();
                }

                transaction.Commit();

                paymentTransactionInfo = new PaymentTransactionInfo()
                {
                    TransactionId = paymentTransaction.Id,
                    TransactionHash = paymentTransaction.Hash,
                    Amount = paymentTransaction.Amount,
                    Currency = request.Currency,
                    GatewayCode = request.GatewayCode,
                    Status = paymentTransaction.Status,
                    IsValid = true
                };
            }

			return paymentTransactionInfo;
        }
    }
}
