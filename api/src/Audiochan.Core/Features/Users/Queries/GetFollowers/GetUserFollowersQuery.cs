﻿using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Common.Mediatr;
using Audiochan.Common.Dtos;
using Audiochan.Common.Extensions;
using Audiochan.Common.Interfaces;
using Audiochan.Core.Features.Users.Mappings;
using Audiochan.Core.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Audiochan.Core.Features.Users.Queries.GetFollowers
{
    public record GetUserFollowersQuery(string Username) : IHasOffsetPage, IQueryRequest<OffsetPagedListDto<FollowerViewModel>>
    {
        public int Offset { get; init; }
        public int Size { get; init; }
    }

    public class GetUserFollowersQueryHandler : IRequestHandler<GetUserFollowersQuery, OffsetPagedListDto<FollowerViewModel>>
    {
        private readonly ApplicationDbContext _dbContext;

        public GetUserFollowersQueryHandler(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<OffsetPagedListDto<FollowerViewModel>> Handle(GetUserFollowersQuery query,
            CancellationToken cancellationToken)
        {
            var list = await _dbContext.FollowedUsers
                .Where(fu => fu.Target.UserName == query.Username)
                .OrderByDescending(fu => fu.FollowedDate)
                .ProjectToFollower()
                .OffsetPaginate(query.Offset, query.Size)
                .ToListAsync(cancellationToken);
            return new OffsetPagedListDto<FollowerViewModel>(list, query.Offset, query.Size);
        }
    }
}