using System;

namespace Audiochan.Core.Entities
{
    public class RefreshToken
    {
        public string Token { get; set; }
        public DateTime Expiry { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Revoked { get; set; }
        public string ReplacedByToken { get; set; }
        public string UserId { get; set; }
    }
}