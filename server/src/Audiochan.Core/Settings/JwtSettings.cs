using System;

namespace Audiochan.Core.Settings
{
    public record JwtSettings
    {
        public string Secret { get; init; }
        public TimeSpan AccessTokenExpiration { get; init; }
        public TimeSpan RefreshTokenExpiration { get; init; }
    }
}