using System.Security.Claims;

namespace Audiochan.Application.Services
{
    public interface ICurrentUserService
    {
        public ClaimsPrincipal? User { get; }
    }
}