namespace GenesisVision.Core.Helpers
{
    public static class Constants
    {
        public static string IpfsHost = "http://localhost:5001";

        #region JWT

        public static string JwtSecretKey { get; set; }

        public static string JwtValidIssuer { get; set; }

        public static string JwtValidAudience { get; set; }

        public static int JwtExpiryInMinutes = 60;

        #endregion
    }
}
