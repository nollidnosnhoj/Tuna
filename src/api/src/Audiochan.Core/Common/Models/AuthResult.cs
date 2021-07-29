namespace Audiochan.Core.Common.Models
{
    public record AuthResult
    {
        public string AccessToken { get; init; } = null!;
        public long AccessTokenExpires { get; init; }
        public string RefreshToken { get; init; } = null!;
        public long RefreshTokenExpires { get; init; }
    }
}