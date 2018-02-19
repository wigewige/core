using GenesisVision.Core.Helpers.Convertors;
using GenesisVision.Core.Models;
using GenesisVision.Core.Services.Interfaces;
using GenesisVision.Core.ViewModels.Wallet;
using GenesisVision.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GenesisVision.Core.Services
{
    public class WalletService : IWalletService
    {
        private readonly ApplicationDbContext context;

        public WalletService(ApplicationDbContext context)
        {
            this.context = context;
        }

        public OperationResult<(List<WalletTransaction>, int)> GetTransactionHistory(Guid userId, TransactionsFilter filter)
        {
            return InvokeOperations.InvokeOperation(() =>
            {
                var query = context.WalletTransactions
                                   .OrderByDescending(x => x.Date)
                                   .Where(x => x.UserId == userId);

                var count = query.Count();

                if (filter.Skip.HasValue)
                    query = query.Skip(filter.Skip.Value);
                if (filter.Take.HasValue)
                    query = query.Take(filter.Take.Value);

                var investments = query.Select(x => x.ToTransaction()).ToList();
                return (investments, count);
            });
        }

        public OperationResult<string> GetUserWallet(Guid userId)
        {
            return InvokeOperations.InvokeOperation(() =>
            {
                var address = context.Wallets.First(w => w.UserId == userId).BlockchainAddresses.FirstOrDefault(x => x.IsDefault)?.Address ??
                              context.Wallets.First(w => w.UserId == userId).BlockchainAddresses.FirstOrDefault()?.Address;
                return address;
            });
        }
    }
}
