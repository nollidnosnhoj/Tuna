using System;
using System.Security.Claims;
using Audiochan.Core.Common.Helpers;
using Audiochan.Core.Interfaces;
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

        public long GetUserId()
        {
            var value = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
#pragma warning disable CA1806
            long.TryParse(value, out long id);
#pragma warning restore CA1806
            return id;
        }

        public bool TryGetUserId(out long userId)
        {
            userId = GetUserId();
            return UserHelpers.IsValidId(userId);
        }

        public bool TryGetUsername(out string username)
        {
            username = GetUsername();
            return !string.IsNullOrEmpty(username);
        }

        public bool IsAuthenticated()
        {
            return TryGetUserId(out _);
        }

        public string GetUsername()
        {
            return _httpContextAccessor.HttpContext?.User.FindFirstValue("name") ?? string.Empty;
        }
    }
}