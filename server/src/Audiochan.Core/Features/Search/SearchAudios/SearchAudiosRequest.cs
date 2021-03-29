using Audiochan.Core.Common.Models.Requests;

namespace Audiochan.Core.Features.Search.SearchAudios
{
    public record SearchAudiosRequest : AudioListQueryRequest
    {
        public string Q { get; init; } = string.Empty;
        public string Tags { get; init; } = string.Empty;
    }
}