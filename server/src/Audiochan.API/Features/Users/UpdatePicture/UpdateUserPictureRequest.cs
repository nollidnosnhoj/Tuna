using Audiochan.Core.Models.Interfaces;
using MediatR;

namespace Audiochan.API.Features.Users.UpdatePicture
{
    public record UpdateUserPictureRequest(string UserId, string ImageData) : IRequest<IResult<string>>
    {
    }
}