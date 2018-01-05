using System.ComponentModel.DataAnnotations;

namespace GenesisVision.Core.ViewModels.Account
{
    public class LoginViewModel
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
