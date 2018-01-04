namespace GenesisVision.Core.Helpers
{
    public static class Constants
    {
        #region Ipfs

        public static string IpfsHost = "http://localhost:5001";

        #endregion

        #region Geth

        public static string GethHost { get; set; }

        #endregion

        #region SendGrid

        public static string SendGridApiKey { get; set; }

        public static string SendGridFromEmail { get; set; }

        public static string SendGridFromName { get; set; }

        #endregion

        #region JWT

        public static string JwtSecretKey { get; set; }

        public static string JwtValidIssuer { get; set; }

        public static string JwtValidAudience { get; set; }

        public static int JwtExpiryInMinutes = 60;

        #endregion
    }
}
