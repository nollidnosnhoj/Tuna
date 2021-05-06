using Audiochan.Core.Common.Models.Responses;
using MediatR;

namespace Audiochan.Core.Features.Audios.GetAudioFeed
{
    public record GetAudioFeedRequest : IRequest<PagedList<AudioViewModel>>
    {
        public string UserId { get; init; }
    }
}