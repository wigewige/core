namespace GenesisVision.Core.Helpers
{
    public static class Constants
    {
        #region Platform

        public static bool IsDevelopment = false;

        public static string UploadPath { get; set; }

        #endregion

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

        public static int JwtExpiryInMinutes { get; set; } = 60;

        #endregion
    }
}
