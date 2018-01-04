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
                .AddClaim("userId", user.Id.ToString())
                .AddClaim("userName", user.UserName)
                .AddIssuer(Constants.JwtValidIssuer)
                .AddAudience(Constants.JwtValidAudience)
                .AddExpiry(Constants.JwtExpiryInMinutes)
                .Build();

            return token;
        }
    }
}
