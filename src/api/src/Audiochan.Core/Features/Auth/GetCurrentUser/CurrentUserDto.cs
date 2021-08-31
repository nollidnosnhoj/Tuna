using Audiochan.Core.Common.Interfaces;

namespace Audiochan.Core.Features.Auth.GetCurrentUser
{
    public record CurrentUserDto : IResourceDto<long>
    {
        public long Id { get; init; }
        public string Username { get; init; } = null!;
        public string Email { get; init; } = null!;
    }
}