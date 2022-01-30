namespace Audiochan.Application.Features.Users.Commands.UpdatePassword
{
    public record UpdatePasswordRequest
    {
        public string CurrentPassword { get; init; } = "";
        public string NewPassword { get; init; } = "";
    }
}