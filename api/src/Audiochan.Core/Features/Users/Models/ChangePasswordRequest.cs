namespace Audiochan.Core.Features.Users.Models
{
    public record ChangePasswordRequest
    {
        public string CurrentPassword { get; init; } = string.Empty;
        public string NewPassword { get; init; } = string.Empty;
    }
}