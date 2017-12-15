namespace GenesisVision.Core.ViewModels.Manager
{
    public class CreateNewManagerWithAccountRequest : CreateNewManagerRequest
    {
        public string Login { get; set; }
        public string Password { get; set; }
    }
}
