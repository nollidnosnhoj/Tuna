using System;

namespace Audiochan.Core.Settings
{
    public record JwtSettings
    {
        public string AccessTokenSecret { get; init; } = null!;
        public string RefreshTokenSecret { get; init; } = null!;
        public TimeSpan AccessTokenExpiration { get; init; }
        public TimeSpan RefreshTokenExpiration { get; init; }
    }
}