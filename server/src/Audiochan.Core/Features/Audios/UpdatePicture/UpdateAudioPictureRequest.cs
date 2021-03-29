using Audiochan.Core.Common.Models.Responses;
using MediatR;

namespace Audiochan.Core.Features.Audios.UpdatePicture
{
    public record UpdateAudioPictureRequest(long Id, string ImageData) : IRequest<IResult<string>>
    {
    }
}