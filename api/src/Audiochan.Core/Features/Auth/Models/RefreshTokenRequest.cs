namespace Audiochan.Core.Features.Auth.Models
{
    public record RefreshTokenRequest
    {
        public string RefreshToken { get; init; }
    }
}