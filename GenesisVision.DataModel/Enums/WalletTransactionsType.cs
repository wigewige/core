namespace GenesisVision.DataModel.Enums
{
    public enum WalletTransactionsType
    {
        // External
        Deposit = 0,
        Withdraw = 1,

        // Internal
        OpenProgram = 2,
        InvestToProgram = 3,
        WithdrawFromProgram = 4,
        ProfitFromProgram = 5,
        CancelInvestmentRequest = 6,
        PartialInvestmentExecutionRefund = 7,
        ClosingProgramRefund = 8
    }
}