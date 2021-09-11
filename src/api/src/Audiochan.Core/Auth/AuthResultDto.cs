namespace Audiochan.Core.Auth
{
    public record AuthResultDto
    {
        public string AccessToken { get; init; } = null!;
        public long AccessTokenExpires { get; init; }
        public string RefreshToken { get; init; } = null!;
        public long RefreshTokenExpires { get; init; }
    }
}