namespace Audiochan.Core.Features.Users.Models
{
    public record UpdateUsernameRequest
    {
        public string Username { get; set; } = string.Empty;
    }
}