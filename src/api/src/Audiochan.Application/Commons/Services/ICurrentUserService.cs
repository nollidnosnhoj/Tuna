using System.Security.Claims;

namespace Audiochan.Application.Commons.Services
{
    public interface ICurrentUserService
    {
        public ClaimsPrincipal? User { get; }
    }
}