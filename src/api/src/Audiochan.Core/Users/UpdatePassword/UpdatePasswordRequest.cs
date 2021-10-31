namespace Audiochan.Core.Users
{
    public record UpdatePasswordRequest
    {
        public string CurrentPassword { get; init; } = "";
        public string NewPassword { get; init; } = "";
    }
}