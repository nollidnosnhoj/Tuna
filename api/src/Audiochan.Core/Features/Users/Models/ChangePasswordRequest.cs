namespace Audiochan.Core.Features.Users.Models
{
    public record ChangePasswordRequest
    {
        public string? CurrentPassword { get; set; } = string.Empty;
        public string? NewPassword { get; set; } = string.Empty;
    }
}