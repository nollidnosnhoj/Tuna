using System.Security.Claims;
using System.Threading.Tasks;
using Audiochan.Core.Auth.GetCurrentUser;
using Audiochan.Core.Common.Extensions;
using Audiochan.Core.Common.Interfaces.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;

namespace Audiochan.API.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public ClaimsPrincipal? User => _httpContextAccessor.HttpContext?.User;
    }
}