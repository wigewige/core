using System;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;

namespace GenesisVision.Core.Helpers.TokenHelper
{
    public sealed class JwtToken
    {
        private readonly SecurityToken token;

        internal JwtToken(SecurityToken token)
        {
            this.token = token;
        }

        public DateTime ValidTo => token.ValidTo;

        public string Value => new JwtSecurityTokenHandler().WriteToken(token);
    }
}
