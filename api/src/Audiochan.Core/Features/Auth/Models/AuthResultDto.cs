using System;

namespace Audiochan.Core.Features.Auth.Models
{
    /// <summary>
    /// Used to return data after being authenticated (eg. access token, refresh token, etc.)
    /// </summary>
    public record AuthResultDto
    {
        public string AccessToken { get; init; } = null!;
        public string RefreshToken { get; init; } = null!;
        public long RefreshTokenExpires { get; init; }
    }
}