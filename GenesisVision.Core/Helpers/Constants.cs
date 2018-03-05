namespace GenesisVision.Core.Helpers
{
    public static class Constants
    {
        #region Platform

        /// <summary>
        /// Is development flag:
        /// autoapprove email;
        /// </summary>
        public static bool IsDevelopment { get; set; } = false;

        /// <summary>
        /// Set investment program period in minutes instead of days
        /// </summary>
        public static bool IsPeriodInMinutes { get; set; } = false;

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
