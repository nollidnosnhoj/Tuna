using System.Threading;
using System.Threading.Tasks;
using Ardalis.Specification;
using Audiochan.Core.Audios;
using Audiochan.Core.Common.Interfaces.Pagination;
using Audiochan.Core.Common.Interfaces.Persistence;
using Audiochan.Core.Common.Interfaces.Services;
using Audiochan.Core.Common.Models.Pagination;
using Audiochan.Domain.Entities;
using MediatR;

namespace Audiochan.Core.Users.GetUserAudios
{
    public record GetUsersAudioQuery(string Username) : IHasOffsetPage, IRequest<OffsetPagedListDto<AudioDto>>
    {
        public int Offset { get; init; } = 1;
        public int Size { get; init; } = 30;
    }

    public sealed class GetUserAudiosSpecification : Specification<Audio>
    {
        public GetUserAudiosSpecification(string username)
        {
            Query.AsNoTracking();
            Query.Where(a => a.User.UserName == username);
            Query.OrderByDescending(a => a.Id);
        }
    }

    public class GetUsersAudioQueryHandler : IRequestHandler<GetUsersAudioQuery, OffsetPagedListDto<AudioDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly long _currentUserId;

        public GetUsersAudioQueryHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _currentUserId = currentUserService.GetUserId();
        }

        public async Task<OffsetPagedListDto<AudioDto>> Handle(GetUsersAudioQuery request,
            CancellationToken cancellationToken)
        {
            var spec = new GetUserAudiosSpecification(request.Username);
            var list = await _unitOfWork.Audios
                .GetOffsetPagedListAsync<AudioDto>(spec, request.Offset, request.Size, cancellationToken);
            return new OffsetPagedListDto<AudioDto>(list, request.Offset, request.Size);
        }
    }
}