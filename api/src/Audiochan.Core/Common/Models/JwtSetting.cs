using System;

namespace Audiochan.Core.Common.Models
{
    public class JwtSetting
    {
        public string Secret { get; set; } = null!;
        public TimeSpan AccessTokenExpiration { get; set; }
        public TimeSpan RefreshTokenExpiration { get; set; }
    }
}