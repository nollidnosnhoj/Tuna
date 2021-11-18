namespace Audiochan.Core.Artists.Commands
{
    public record UpdateProfileRequest
    {
        public string? DisplayName { get; init; }
    }
}