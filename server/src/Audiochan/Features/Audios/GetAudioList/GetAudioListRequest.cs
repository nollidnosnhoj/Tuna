using Audiochan.Core.Models.Interfaces;
using Audiochan.Core.Models.Responses;
using Audiochan.Core.Models.ViewModels;
using MediatR;

namespace Audiochan.Features.Audios.GetAudioList
{
    public record GetAudioListRequest : IHasCursor<long>, IRequest<CursorList<AudioViewModel, long>>
    {
        public long? Cursor { get; init; }
        public int Size { get; init; } = 30;
    }
}