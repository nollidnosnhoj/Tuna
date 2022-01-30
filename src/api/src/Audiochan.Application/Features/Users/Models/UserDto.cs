using Audiochan.Application.Commons.Interfaces;
using Audiochan.Domain.Entities;
using AutoMapper;

namespace Audiochan.Application.Features.Users.Models
{
    public record UserDto : IMapFrom<User>
    {
        public long Id { get; init; }
        
        public string UserName { get; init; } = null!;
        
        public string? Picture { get; init; }
    }

    public record FollowedUserDto : IMapFrom<FollowedUser>
    {
        public long ObserverId { get; init; }
        public UserDto Observer { get; init; }
        public long TargetId { get; init; }
        public UserDto Target { get; init; }
    }
}