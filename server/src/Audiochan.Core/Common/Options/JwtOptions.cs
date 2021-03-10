using System;

namespace Audiochan.Core.Common.Options
{
    public record JwtOptions
    {
        public string Secret { get; init; }
        public TimeSpan AccessTokenExpiration { get; init; }
        public TimeSpan RefreshTokenExpiration { get; init; }
    }
}