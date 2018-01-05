using GenesisVision.DataModel.Models;

namespace GenesisVision.Core.Helpers.TokenHelper
{
    public class JwtManager
    {
        public static JwtToken GenerateToken(ApplicationUser user)
        {
            var token = new JwtTokenBuilder()
                .AddSecurityKey(JwtSecurityKey.Create(Constants.JwtSecretKey))
                .AddSubject(user.UserName)
                .AddClaim("id", user.Id.ToString())
                .AddIssuer(Constants.JwtValidIssuer)
                .AddAudience(Constants.JwtValidAudience)
                .AddExpiry(Constants.JwtExpiryInMinutes)
                .Build();

            return token;
        }
    }
}
