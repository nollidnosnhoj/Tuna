using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Extensions;
using Audiochan.Core.Common.Extensions.MappingExtensions;
using Audiochan.Core.Common.Interfaces;
using Audiochan.Core.Common.Models.Interfaces;
using Audiochan.Core.Common.Models.Responses;
using Audiochan.Core.Common.Settings;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Audiochan.Core.Features.Followers.GetFollowers
{
    public record GetUserFollowersRequest : IHasPage, IRequest<PagedList<FollowerViewModel>>
    {
        public string Username { get; init; } = string.Empty;
        public int Page { get; init; }
        public int Size { get; init; }
    }

    public class GetUserFollowersRequestHandler : IRequestHandler<GetUserFollowersRequest, PagedList<FollowerViewModel>>
    {
        private readonly IApplicationDbContext _dbContext;
        private readonly MediaStorageSettings _storageSettings;
        private readonly IFollowedUserRepository _followedUserRepository;

        public GetUserFollowersRequestHandler(IApplicationDbContext dbContext, IOptions<MediaStorageSettings> options, IFollowedUserRepository followedUserRepository)
        {
            _dbContext = dbContext;
            _followedUserRepository = followedUserRepository;
            _storageSettings = options.Value;
        }

        public async Task<PagedList<FollowerViewModel>> Handle(GetUserFollowersRequest request,
            CancellationToken cancellationToken)
        {
            return await _followedUserRepository.GetPagedListBySpec(new GetUserFollowersSpecification(request.Username),
                request.Page, request.Size, cancellationToken: cancellationToken);
        }
    }
}