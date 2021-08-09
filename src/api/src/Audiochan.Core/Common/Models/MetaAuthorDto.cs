using Audiochan.Core.Common.Constants;
using Audiochan.Core.Entities;

namespace Audiochan.Core.Common.Models
{
    public record MetaAuthorDto
    {
        public MetaAuthorDto(User user)
        {
            Id = user.Id;
            Username = user.UserName;
            Picture = user.Picture != null
                ? string.Format(MediaLinkInvariants.UserPictureUrl, user.Picture)
                : null;
        }
        
        public long Id { get; init; }
        public string Username { get; init; } = null!;
        public string? Picture { get; init; }
    }
}