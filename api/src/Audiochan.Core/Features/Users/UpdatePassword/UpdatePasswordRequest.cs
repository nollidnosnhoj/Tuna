namespace Audiochan.Core.Features.Users.UpdatePassword
{
    public record UpdatePasswordRequest
    {
        public string CurrentPassword { get; init; } = "";
        public string NewPassword { get; init; } = "";
    }
}