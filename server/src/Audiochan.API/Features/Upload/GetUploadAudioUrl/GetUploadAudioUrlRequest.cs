using MediatR;

namespace Audiochan.API.Features.Upload.GetUploadAudioUrl
{
    public record GetUploadAudioUrlRequest : IRequest<GetUploadAudioUrlResponse>
    {
        public string FileName { get; init; }
        public long FileSize { get; init; }
    }
}