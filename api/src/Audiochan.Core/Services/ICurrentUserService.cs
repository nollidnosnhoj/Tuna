using System.Security.Claims;

namespace Audiochan.Core.Services
{
    public interface ICurrentUserService
    {
        public ClaimsPrincipal? User { get; }
    }
}