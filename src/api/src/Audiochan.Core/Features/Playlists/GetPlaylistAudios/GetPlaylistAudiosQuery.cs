﻿using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Interfaces.Pagination;
using Audiochan.Core.Common.Models.Pagination;
using Audiochan.Core.Features.Audios;
using Audiochan.Core.Interfaces;
using Audiochan.Core.Interfaces.Persistence;
using MediatR;

namespace Audiochan.Core.Features.Playlists.GetPlaylistAudios
{
    public record GetPlaylistAudiosQuery(long Id) : IHasCursorPage<long>, IRequest<CursorPagedListDto<AudioDto, long>>
    {
        public long Cursor { get; init; }
        public int Size { get; init; }
    }

    public class GetPlaylistAudiosQueryHandler : IRequestHandler<GetPlaylistAudiosQuery, CursorPagedListDto<AudioDto, long>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly long _currentUserId;

        public GetPlaylistAudiosQueryHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _currentUserId = currentUserService.GetUserId();
        }

        public async Task<CursorPagedListDto<AudioDto, long>> Handle(GetPlaylistAudiosQuery request, CancellationToken cancellationToken)
        {
            var results = await _unitOfWork.Audios.GetPlaylistAudios(request, cancellationToken);
            return new CursorPagedListDto<AudioDto, long>(results, request.Size);
        }
    }
}