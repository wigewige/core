namespace GenesisVision.Core.Services.Interfaces
{
    public interface IRateService
    {
        decimal GetRate(string from, string to);
    }
}
