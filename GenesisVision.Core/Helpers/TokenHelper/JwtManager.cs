using GenesisVision.DataModel.Models;

namespace GenesisVision.Core.Helpers.TokenHelper
{
    public class JwtManager
    {
        public static JwtToken GenerateToken(ApplicationUser user)
        {
            var token = new JwtTokenBuilder()
                .AddSecurityKey(JwtSecurityKey.Create(Constants.SecretKey))
                .AddSubject("UserName")
                .AddIssuer("GenesisVision.Core")
                .AddAudience("GenesisVision.Core")
                .AddExpiry(1)
                .Build();

            return token;
        }
    }
}
