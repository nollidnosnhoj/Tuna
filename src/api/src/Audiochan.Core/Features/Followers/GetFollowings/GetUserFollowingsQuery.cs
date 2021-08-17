﻿using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Extensions;
using Audiochan.Core.Common.Interfaces;
using Audiochan.Core.Common.Models;
using Audiochan.Core.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Audiochan.Core.Features.Followers.GetFollowings
{
    public record GetUserFollowingsQuery : IHasOffsetPage, IRequest<OffsetPagedListDto<FollowingViewModel>>
    {
        public string Username { get; init; } = string.Empty;
        public int Offset { get; init; }
        public int Size { get; init; }
    }

    public class GetUserFollowingsQueryHandler : IRequestHandler<GetUserFollowingsQuery, OffsetPagedListDto<FollowingViewModel>>
    {
        private readonly ApplicationDbContext _unitOfWork;

        public GetUserFollowingsQueryHandler(ApplicationDbContext unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<OffsetPagedListDto<FollowingViewModel>> Handle(GetUserFollowingsQuery query,
            CancellationToken cancellationToken)
        {
            return await _unitOfWork.Users
                .AsNoTracking()
                .Where(u => u.UserName == query.Username)
                .SelectMany(u => u.Followings)
                .OrderByDescending(fu => fu.FollowedDate)
                .Select(FollowedUserMaps.UserToFollowingFunc)
                .OffsetPaginateAsync(query, cancellationToken);
        }
    }
}