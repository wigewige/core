namespace GenesisVision.DataModel.Enums
{
    public enum PaymentTransactionStatus
    {
        Undefined = 0,
        New = 10,
        Pending = 100,
        ConfirmedByGate = 200,
        ConfirmedAndValidated = 250,
        Error = 500,
        Canceled = 550
    }
}
