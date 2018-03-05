using GenesisVision.Common.Models;
using GenesisVision.Core.Helpers.Convertors;
using GenesisVision.Core.Services.Interfaces;
using GenesisVision.Core.ViewModels.Wallet;
using GenesisVision.DataModel;
using GenesisVision.DataModel.Enums;
using GenesisVision.DataModel.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using MoreLinq;

namespace GenesisVision.Core.Services
{
    public class WalletService : IWalletService
    {
        private readonly ApplicationDbContext context;
        private readonly ILogger<IWalletService> logger;

        private readonly List<WalletTransactionsType> internalTxType;
        private readonly List<WalletTransactionsType> externalTxType;

        public WalletService(ApplicationDbContext context, ILogger<IWalletService> logger)
        {
            this.context = context;
            this.logger = logger;

            externalTxType = new List<WalletTransactionsType>
                             {
                                 WalletTransactionsType.Deposit,
                                 WalletTransactionsType.Withdraw
                             };
            internalTxType = new List<WalletTransactionsType>();
            foreach (WalletTransactionsType x in Enum.GetValues(typeof(WalletTransactionsType)))
            {
                if (!externalTxType.Contains(x))
                    continue;

                internalTxType.Add(x);
            }
        }

        public OperationResult<(List<WalletTransaction>, int)> GetTransactionHistory(Guid userId, TransactionsFilter filter)
        {
            return InvokeOperations.InvokeOperation(() =>
            {
                var query = context.WalletTransactions
                                   .Include(x => x.Wallet)
                                   .Include(x => x.PaymentTransaction)
                                   .ThenInclude(x => x.BlockchainAddress)
                                   .Include(x => x.InvestmentRequest)
                                   .ThenInclude(x => x.InvestmentProgram)
                                   .OrderByDescending(x => x.Date)
                                   .Where(x => x.Wallet.UserId == userId);

                var count = query.Count();

                if (filter.Skip.HasValue)
                    query = query.Skip(filter.Skip.Value);
                if (filter.Take.HasValue)
                    query = query.Take(filter.Take.Value);
                if (filter.InvestmentProgramId.HasValue)
                    query = query.Where(x => x.InvestmentRequest != null &&
                                             x.InvestmentRequest.InvestmentProgramtId == filter.InvestmentProgramId.Value);
                if (filter.Type.HasValue)
                {
                    switch (filter.Type.Value)
                    {
                        case WalletTxType.Internal:
                            query = query.Where(x => internalTxType.Contains(x.Type));
                            break;
                        case WalletTxType.External:
                            query = query.Where(x => externalTxType.Contains(x.Type));
                            break;
                    }
                }

                var investments = query.Select(x => x.ToTransaction()).ToList();
                return (investments, count);
            });
        }

        public OperationResult<WalletAddressViewModel> GetUserDefaultBlockchainAddress(Guid userId)
        {
            return InvokeOperations.InvokeOperation(() =>
            {
                var address = context.BlockchainAddresses.FirstOrDefault(x => x.UserId == userId && x.IsDefault) ??
                              context.BlockchainAddresses.FirstOrDefault(x => x.UserId == userId);

                var result = new WalletAddressViewModel
                             {
                                 Address = address?.Address,
                                 Currency = address?.Currency ?? Currency.Undefined
                             };
                return result;
            });
        }

        public OperationResult WithdrawRequest(WalletWithdrawRequestModel request, Guid userId)
        {
            return InvokeOperations.InvokeOperation(() =>
            {
                var wallet = context.Wallets.First(w => w.UserId == userId && w.Currency == request.Currency);
                if (wallet.Amount < request.Amount)
                    throw new ApplicationException("Not enough funds");

                // TODO wallet relation
                var transaction = new PaymentTransactions
                {
                    Id = Guid.NewGuid(),
                    Amount = request.Amount,
                    Type = PaymentTransactionType.Withdrawn,
                    DateCreated = DateTime.UtcNow,
                    DestAddress = request.BlockchainAddress,
                    Status = PaymentTransactionStatus.New
                };

                context.Add(transaction);
            });
        }

        public OperationResult<WalletsViewModel> GetUserWallets(Guid userId)
        {
            return InvokeOperations.InvokeOperation(() =>
            {
                var wallets = context.Wallets.Where(w => w.UserId == userId);
                return new WalletsViewModel
                       {
                           Wallets = wallets.Select(x => x.ToWallet()).ToList()
                       };
            });
        }

        public OperationResult<WalletInvestmentPrograms> GetInvestmentProgramsWithTx(string mask, Guid userId)
        {
            return InvokeOperations.InvokeOperation(() =>
            {
                var query = context.WalletTransactions
                                   .Include(x => x.InvestmentRequest)
                                   .ThenInclude(x => x.InvestmentProgram)
                                   .Where(x => x.Wallet.UserId == userId && x.InvestmentRequest != null);

                if (!string.IsNullOrEmpty(mask))
                {
                    mask = mask.Trim().ToLower();
                    query = query.Where(x => x.InvestmentRequest.InvestmentProgram.Title.ToLower().Contains(mask));
                }

                var res = query
                          .DistinctBy(x => x.InvestmentRequest.InvestmentProgramtId)
                          .Select(x => new WalletInvestmentProgram
                                       {
                                           Id = x.InvestmentRequest.InvestmentProgram.Id,
                                           Title = x.InvestmentRequest.InvestmentProgram.Title
                                       })
                          .ToList();
                return new WalletInvestmentPrograms {InvestmentPrograms = res};
            });
        }
    }
}
