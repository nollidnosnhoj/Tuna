using System;

namespace Audiochan.Domain.Entities
{
    public class RefreshToken
    {
        public string Token { get; set; } = null!;
        public DateTime Expiry { get; set; }
        public DateTime Created { get; set; }
        public long UserId { get; set; }
    }
}