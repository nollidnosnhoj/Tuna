using Audiochan.Core.Models.Interfaces;
using Audiochan.Core.Models.Responses;
using Audiochan.Core.Models.ViewModels;
using MediatR;

namespace Audiochan.API.Features.Users.GetUserAudios
{
    public record GetUserAudiosRequest : IHasPage, IRequest<PagedList<AudioViewModel>>
    {
        public string Username { get; init; }
        public int Page { get; init; }
        public int Size { get; init; }
    }
}