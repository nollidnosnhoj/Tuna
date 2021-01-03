using System;

namespace Audiochan.Core.Features.Auth.Models
{
    /// <summary>
    /// Used to return data after being authenticated (eg. access token, refresh token, etc.)
    /// </summary>
    public class AuthResultDto
    {
        public string AccessToken { get; set; } = null!;
        public string RefreshToken { get; set; } = null!;
        public long RefreshTokenExpires { get; set; }
    }
}