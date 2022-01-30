using Audiochan.Application.Commons.Interfaces;
using Audiochan.Domain.Entities;

namespace Audiochan.Application.Features.Users.Models
{
    public record UserDto : IMapFrom<User>
    {
        public long Id { get; init; }
        
        public string UserName { get; init; } = null!;
        
        public string? Picture { get; init; }
    }
}