using Audiochan.Core.Models.Responses;
using Audiochan.Core.Models.ViewModels;
using MediatR;

namespace Audiochan.Core.Features.Audios.GetAudioFeed
{
    public record GetAudioFeedRequest : IRequest<PagedList<AudioViewModel>>
    {
        public string UserId { get; init; }
    }
}