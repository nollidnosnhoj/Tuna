using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.API.Features.Audios.Mappings;
using Audiochan.Core.CQRS;
using Audiochan.Core.Dtos;
using Audiochan.Core.Dtos.Wrappers;
using Audiochan.Core.Extensions;
using Audiochan.Core.Persistence;
using Audiochan.Core.Services;
using MediatR;

namespace Audiochan.Core.Users.Queries
{
    public record GetUsersAudioQuery(string Username) : IQueryRequest, IRequest<OffsetPagedListDto<AudioDto>>
    {
        public int Offset { get; init; } = 1;
        public int Size { get; init; } = 30;
    }

    public class GetUsersAudioQueryHandler : IRequestHandler<GetUsersAudioQuery, OffsetPagedListDto<AudioDto>>
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ICurrentUserService _currentUserService;

        public GetUsersAudioQueryHandler(ApplicationDbContext dbContext, ICurrentUserService currentUserService)
        {
            _dbContext = dbContext;
            _currentUserService = currentUserService;
        }

        public async Task<OffsetPagedListDto<AudioDto>> Handle(GetUsersAudioQuery request,
            CancellationToken cancellationToken)
        {
            _currentUserService.User.TryGetUserId(out var userId);
            var list = await _dbContext.Users
                .Where(u => u.UserName == request.Username)
                .SelectMany(u => u.Audios)
                .OrderByDescending(a => a.Id)
                .Project(userId)
                .OffsetPaginateAsync(request.Offset, request.Size, cancellationToken);
            return new OffsetPagedListDto<AudioDto>(list, request.Offset, request.Size);
        }
    }
}