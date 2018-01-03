namespace GenesisVision.Core.Helpers.TokenHelper
{
    public class JwtManager
    {
        public static JwtToken GenerateToken()
        {
            var token = new JwtTokenBuilder()
                .AddSecurityKey(JwtSecurityKey.Create(Constants.SecretKey))
                .AddSubject("UserName")
                .AddIssuer("GenesisVision.Core")
                .AddAudience("GenesisVision.Core")
                .AddExpiry(60)
                .Build();

            return token;
        }
    }
}
