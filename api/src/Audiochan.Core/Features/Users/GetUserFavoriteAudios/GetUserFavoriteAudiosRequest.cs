using System.Linq;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Extensions;
using Audiochan.Core.Common.Interfaces;
using Audiochan.Core.Common.Mappings;
using Audiochan.Core.Common.Models;
using Audiochan.Core.Features.Audios.GetAudioList;
using Audiochan.Core.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Audiochan.Core.Features.Users.GetUserFavoriteAudios
{
    public record GetUserFavoriteAudiosRequest : IHasPage, IRequest<PagedListDto<AudioViewModel>>
    {
        [JsonIgnore] public string? Username { get; set; }
        public int Page { get; init; } = 1;
        public int Size { get; init; } = 30;
    }
    
    public class GetUserFavoriteAudiosRequestHandler : IRequestHandler<GetUserFavoriteAudiosRequest, PagedListDto<AudioViewModel>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public GetUserFavoriteAudiosRequestHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        public async Task<PagedListDto<AudioViewModel>> Handle(GetUserFavoriteAudiosRequest request, CancellationToken cancellationToken)
        {
            return await _unitOfWork.Users
                .AsNoTracking()
                .Include(u => u.FavoriteAudios)
                .ThenInclude(fa => fa.Audio)
                .Where(u => u.UserName == request.Username)
                .SelectMany(u => u.FavoriteAudios)
                .OrderByDescending(fa => fa.FavoriteDate)
                .Select(fa => fa.Audio)
                .Select(AudioMappings.AudioToListProjection())
                .PaginateAsync(request, cancellationToken);
        }
    }
}