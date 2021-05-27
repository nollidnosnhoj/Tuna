using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Repositories;
using Audiochan.Core.Services;
using MediatR;

namespace Audiochan.API.Features.Users.GetProfile
{
    public record GetProfileRequest(string Username) : IRequest<ProfileViewModel?>
    {
    }

    public class GetProfileRequestHandler : IRequestHandler<GetProfileRequest, ProfileViewModel?>
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IUnitOfWork _unitOfWork;

        public GetProfileRequestHandler(ICurrentUserService currentUserService, IUnitOfWork unitOfWork)
        {
            _currentUserService = currentUserService;
            _unitOfWork = unitOfWork;
        }

        public async Task<ProfileViewModel?> Handle(GetProfileRequest request, CancellationToken cancellationToken)
        {
            var currentUserId = _currentUserService.GetUserId();

            return await _unitOfWork.Users.GetAsync(new GetProfileSpecification(request.Username, currentUserId),
                cancellationToken: cancellationToken);
        }
    }
}