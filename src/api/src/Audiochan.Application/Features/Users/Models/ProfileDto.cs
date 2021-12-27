using Audiochan.Application.Commons.Interfaces;
using Audiochan.Domain.Abstractions;
using Audiochan.Domain.Entities;

namespace Audiochan.Application.Features.Users.Models
{
    public record ProfileDto : IHasId<long>, IMapFrom<User>
    {
        public long Id { get; init; }
        public string UserName { get; init; } = string.Empty;
        public string? Picture { get; init; }
    }
}