using GenesisVision.Core.ViewModels.Wallet;
using GenesisVision.DataModel.Models;

namespace GenesisVision.Core.Helpers.Convertors
{
    public static partial class Convertors
    {
        public static WalletTransaction ToTransaction(this WalletTransactions tx)
        {
            return new WalletTransaction
                   {
                       Id = tx.Id,
                       Type = tx.Type,
                       Date = tx.Date,
                       Amount = tx.Amount,
                       WalletId = tx.Wallet.Id,
                       Currency = tx.Wallet.Currency,
                       InvestmentProgramRequest = tx.InvestmentRequest == null
                           ? null
                           : new InvestmentProgramRequestTxInfo
                             {
                                 Status = tx.InvestmentRequest.Status,
                                 Type = tx.InvestmentRequest.Type,
                                 InvestmentProgram = new InvestmentProgramTxInfo
                                                     {
                                                         Id = tx.InvestmentRequest.InvestmentProgramtId,
                                                         Title = tx.InvestmentRequest.InvestmentProgram.Title
                                                     }
                             },
                       PaymentTx = tx.PaymentTransaction == null
                           ? null
                           : new PaymentTxInfo
                             {
                                 Id = tx.PaymentTransaction.Id,
                                 Hash = tx.PaymentTransaction.Hash,
                                 Address = tx.PaymentTransaction.BlockchainAddress.Address
                             }
                   };
        }
    }
}
