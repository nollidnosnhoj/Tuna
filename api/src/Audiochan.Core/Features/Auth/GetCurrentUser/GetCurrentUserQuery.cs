using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Interfaces;
using Audiochan.Core.Common.Mappings;
using Audiochan.Core.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Audiochan.Core.Features.Auth.GetCurrentUser
{
    public record GetCurrentUserQuery : IRequest<CurrentUserViewModel?>
    {
    }

    public class GetCurrentUserQueryHandler : IRequestHandler<GetCurrentUserQuery, CurrentUserViewModel?>
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IUnitOfWork _unitOfWork;

        public GetCurrentUserQueryHandler(ICurrentUserService currentUserService, IUnitOfWork unitOfWork)
        {
            _currentUserService = currentUserService;
            _unitOfWork = unitOfWork;
        }

        public async Task<CurrentUserViewModel?> Handle(GetCurrentUserQuery query,
            CancellationToken cancellationToken)
        {
            var currentUserId = _currentUserService.GetUserId();

            return await _unitOfWork.Users
                .AsNoTracking()
                .Where(u => u.Id == currentUserId)
                .ProjectToCurrentUser()
                .SingleOrDefaultAsync(cancellationToken);
        }
    }
}