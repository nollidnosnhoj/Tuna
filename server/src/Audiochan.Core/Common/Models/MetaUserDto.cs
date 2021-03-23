using Audiochan.Core.Entities;

namespace Audiochan.Core.Common.Models
{
    public record MetaUserDto
    {
        public MetaUserDto(User user)
        {
            this.Id = user.Id;
            this.Username = user.UserName;
            this.Picture = user.Picture;

        }
        
        public string Id { get; init; }
        public string Username { get; init; }
        public string Picture { get; init; }
    }
}