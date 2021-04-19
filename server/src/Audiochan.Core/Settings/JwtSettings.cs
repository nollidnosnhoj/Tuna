using System;

namespace Audiochan.Core.Settings
{
    public record JwtSettings
    {
        public string AccessTokenSecret { get; init; }
        public string RefreshTokenSecret { get; init; }
        public TimeSpan AccessTokenExpiration { get; init; }
        public TimeSpan RefreshTokenExpiration { get; init; }
    }
}