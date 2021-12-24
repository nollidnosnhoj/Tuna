using System.Security.Claims;

namespace Audiochan.Core.Commons.Services
{
    public interface ICurrentUserService
    {
        public ClaimsPrincipal? User { get; }
    }
}