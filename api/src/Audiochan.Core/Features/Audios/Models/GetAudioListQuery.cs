using Audiochan.Core.Common.Models;

namespace Audiochan.Core.Features.Audios.Models
{
    public record GetAudioListQuery : PaginationQuery
    {
        public string Username { get; init; } = string.Empty;
        public string Tags { get; init; } = string.Empty;
        public string Sort { get; init; } = string.Empty;
    }
}