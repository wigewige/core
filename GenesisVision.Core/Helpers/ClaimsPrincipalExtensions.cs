using System;
using System.Security.Claims;

namespace GenesisVision.Core.Helpers
{
    public static class ClaimsPrincipalExtensions
    {
        public static Guid? GetUserId(this ClaimsPrincipal principal)
        {
            if (principal == null)
                throw new ArgumentNullException(nameof(principal));

            var id = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            return !string.IsNullOrEmpty(id) && Guid.TryParse(id, out var userId)
                ? (Guid?)userId
                : null;
        }
    }
}
