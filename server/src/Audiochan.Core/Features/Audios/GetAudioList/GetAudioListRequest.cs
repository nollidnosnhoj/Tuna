using Audiochan.Core.Common.Models.Interfaces;
using Audiochan.Core.Common.Models.Responses;
using MediatR;

namespace Audiochan.Core.Features.Audios.GetAudioList
{
    public record GetAudioListRequest : IHasCursor<long>, IRequest<CursorList<AudioViewModel, long>>
    {
        public long? Cursor { get; init; }
        public int Size { get; init; } = 30;
    }
}