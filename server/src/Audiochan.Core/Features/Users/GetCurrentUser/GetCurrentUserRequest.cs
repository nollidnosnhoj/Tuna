using Audiochan.Core.Common.Models.Responses;
using Audiochan.Core.Common.Models.ViewModels;
using MediatR;

namespace Audiochan.Core.Features.Users.GetCurrentUser
{
    public record GetCurrentUserRequest : IRequest<CurrentUserViewModel>
    {
    }
}