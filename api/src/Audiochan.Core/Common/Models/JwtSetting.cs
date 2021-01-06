using System;

namespace Audiochan.Core.Common.Models
{
    public record JwtSetting
    {
        public string Secret { get; init; } = null!;
        public TimeSpan AccessTokenExpiration { get; init; }
        public TimeSpan RefreshTokenExpiration { get; init; }
    }
}