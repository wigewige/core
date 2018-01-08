using GenesisVision.Core.Helpers;
using GenesisVision.Core.Helpers.TokenHelper;
using GenesisVision.DataModel.Models;
using JWT;
using JWT.Serializers;
using NUnit.Framework;
using System;

namespace GenesisVision.Core.Tests.Helpers
{
    [TestFixture]
    public class JwtTests
    {
        private ApplicationUser user;
        private JwtDecoder jwtDecoder;
        
        [SetUp]
        public void Init()
        {
            var serializer = new JsonNetSerializer();
            var validator = new JwtValidator(serializer, new UtcDateTimeProvider());
            jwtDecoder = new JwtDecoder(serializer, validator, new JwtBase64UrlEncoder());

            Constants.JwtValidIssuer = "TestIssuer";
            Constants.JwtValidAudience = "TestAudience";
            Constants.JwtSecretKey = "test-gv-core-secret-key";
            Constants.JwtExpiryInMinutes = 10;

            user = new ApplicationUser
                   {
                       Id = Guid.NewGuid(),
                       UserName = "Test username"
                   };
        }

        [Test]
        public void JwtFieldsTest()
        {
            var token = JwtManager.GenerateToken(user);
            
            var decodedToken = jwtDecoder.DecodeToObject(token.Value);

            Assert.IsTrue(decodedToken.ContainsKey("aud"));
            Assert.AreEqual(Constants.JwtValidAudience, decodedToken["aud"].ToString());

            Assert.IsTrue(decodedToken.ContainsKey("iss"));
            Assert.AreEqual(Constants.JwtValidIssuer, decodedToken["iss"].ToString());

            Assert.IsTrue(decodedToken.ContainsKey("unique_name"));
            Assert.AreEqual(user.UserName, decodedToken["unique_name"].ToString());

            Assert.IsTrue(decodedToken.ContainsKey("id"));
            Assert.AreEqual(user.Id.ToString(), decodedToken["id"].ToString());
        }

        [Test]
        public void JwtValidateSecretKey()
        {
            var token = JwtManager.GenerateToken(user);
            var decodedToken = "";

            decodedToken = jwtDecoder.Decode(token.Value, Constants.JwtSecretKey, false);
            Assert.IsNotEmpty(decodedToken);

            jwtDecoder.Decode(token.Value, Constants.JwtSecretKey, true);
            Assert.IsNotEmpty(decodedToken);

            decodedToken = jwtDecoder.Decode(token.Value, "wrong-secret-key", false);
            Assert.IsNotEmpty(decodedToken);

            var ex = Assert.Throws<SignatureVerificationException>(() =>
            {
                jwtDecoder.Decode(token.Value, "wrong-secret-key", true);
            });
            Assert.AreEqual("Invalid signature", ex.Message);
        }

        [Test]
        [TestCase(1)]
        [TestCase(256)]
        [TestCase(1024)]
        public void JwtValidateDate(int minutesCount)
        {
            Constants.JwtExpiryInMinutes = minutesCount;

            var token = JwtManager.GenerateToken(user);
            var dateTo = DateTime.UtcNow.AddMinutes(minutesCount);

            Assert.IsTrue(Math.Abs((dateTo - token.ValidTo).TotalSeconds) < 3);
        }
    }
}
