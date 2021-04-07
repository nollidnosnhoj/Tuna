using Audiochan.Core.Common.Models.Interfaces;
using Audiochan.Core.Common.Models.Responses;
using MediatR;

namespace Audiochan.Core.Features.Users.UpdatePicture
{
    public record UpdateUserPictureRequest(string UserId, string ImageData) : IRequest<IResult<string>>
    {
    }
}