using Audiochan.Core.Common.Models.Requests;
using Audiochan.Core.Common.Models.Responses;
using Audiochan.Core.Features.Audios.GetAudio;
using MediatR;

namespace Audiochan.Core.Features.Audios.CreateAudio
{
    public record CreateAudioRequest : AudioAbstractRequest, IRequest<Result<AudioDetailViewModel>>
    {
        public string UploadId { get; init; }
        public string FileName { get; init; }
        public long FileSize { get; init; }
        public int Duration { get; init; }
    }
}