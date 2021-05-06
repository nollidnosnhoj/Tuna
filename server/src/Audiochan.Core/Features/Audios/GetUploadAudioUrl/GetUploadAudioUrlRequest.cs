using MediatR;

namespace Audiochan.Core.Features.Audios.GetUploadAudioUrl
{
    public record GetUploadAudioUrlRequest : IRequest<GetUploadAudioUrlResponse>
    {
        public string FileName { get; init; }
        public long FileSize { get; init; }
    }
}