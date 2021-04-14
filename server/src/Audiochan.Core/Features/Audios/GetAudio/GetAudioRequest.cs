using Audiochan.Core.Common.Models.Responses;
using Audiochan.Core.Common.Models.ViewModels;
using MediatR;

namespace Audiochan.Core.Features.Audios.GetAudio
{
    public record GetAudioRequest(long Id, string PrivateKey = "") : IRequest<AudioDetailViewModel>
    {
    }
}