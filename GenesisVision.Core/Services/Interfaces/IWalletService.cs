using GenesisVision.Core.Models;
using GenesisVision.Core.ViewModels.Wallet;
using System;
using System.Collections.Generic;

namespace GenesisVision.Core.Services.Interfaces
{
    public interface IWalletService
    {
        OperationResult<(List<WalletTransaction>, int)> GetTransactionHistory(Guid userId, TransactionsFilter filter);

        OperationResult<WalletAddressViewModel> GetUserDefaultBlockchainAddress(Guid userId);

        OperationResult WithdrawRequest(WalletWithdrawRequestModel request, Guid userId);

        OperationResult<WalletsViewModel> GetUserWallets(Guid userId);
    }
}
