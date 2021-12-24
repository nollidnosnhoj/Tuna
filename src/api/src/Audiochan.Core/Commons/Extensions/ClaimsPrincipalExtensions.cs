using System.Security.Claims;
using Audiochan.Core.Commons.Exceptions;

namespace Audiochan.Core.Commons.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static bool TryGetUserId(this ClaimsPrincipal? principal, out long userId)
        {
            var claim = principal?.FindFirst(ClaimTypes.NameIdentifier);

            if (claim is not null)
            {
                return long.TryParse(claim.Value, out userId);
            }
            
            userId = 0;
            return false;
        }
        
        public static bool TryGetUserName(this ClaimsPrincipal? principal, out string userName)
        {
            var claim = principal?.FindFirst(ClaimTypes.Name);

            if (claim is not null && !string.IsNullOrWhiteSpace(claim.Value))
            {
                userName = claim.Value;
                return true;
            }

            userName = "";
            return false;
        }

        public static long GetUserId(this ClaimsPrincipal? principal)
        {
            if (!principal.TryGetUserId(out var userId))
            {
                throw new UnauthorizedException();
            }

            return userId;
        }

        public static string GetUserName(this ClaimsPrincipal? principal)
        {
            if (!principal.TryGetUserName(out var userName))
            {
                throw new UnauthorizedException();
            }

            return userName;
        }
    }
}