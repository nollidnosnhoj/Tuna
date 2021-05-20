using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Extensions.MappingExtensions;
using Audiochan.Core.Common.Interfaces;
using Audiochan.Core.Common.Settings;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Audiochan.Core.Features.Users.GetUser
{
    public record GetUserRequest(string Username) : IRequest<UserViewModel>
    {
    }

    public class GetUserRequestHandler : IRequestHandler<GetUserRequest, UserViewModel>
    {
        private readonly IApplicationDbContext _dbContext;
        private readonly ICurrentUserService _currentUserService;
        private readonly MediaStorageSettings _storageSettings;

        public GetUserRequestHandler(ICurrentUserService currentUserService, IApplicationDbContext dbContext, IOptions<MediaStorageSettings> options)
        {
            _currentUserService = currentUserService;
            _dbContext = dbContext;
            _storageSettings = options.Value;
        }

        public async Task<UserViewModel> Handle(GetUserRequest request, CancellationToken cancellationToken)
        {
            var currentUserId = _currentUserService.GetUserId();

            return await _dbContext.Users
                .AsNoTracking()
                .Include(u => u.Followers)
                .Include(u => u.Followings)
                .Include(u => u.Audios)
                .Where(u => u.UserName == request.Username.Trim().ToLower())
                .ProjectToUser(currentUserId, _storageSettings)
                .SingleOrDefaultAsync(cancellationToken);
        }
    }
}