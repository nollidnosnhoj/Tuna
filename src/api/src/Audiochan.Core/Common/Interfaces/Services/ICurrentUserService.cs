using System.Security.Claims;

namespace Audiochan.Core.Common.Interfaces.Services
{
    public interface ICurrentUserService
    {
        public ClaimsPrincipal? User { get; }
    }
}