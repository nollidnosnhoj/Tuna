using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Extensions;
using Audiochan.Core.Common.Interfaces;
using Audiochan.Core.Common.Mappings;
using Audiochan.Core.Common.Models;
using Audiochan.Core.Features.Audios.GetAudioList;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Audiochan.Core.Features.Audios.SearchAudios
{
    public record SearchAudiosRequest : IHasPage, IRequest<PagedListDto<AudioViewModel>>
    {
        public string Q { get; init; } = string.Empty;
        public string Tags { get; init; } = string.Empty;
        public int Page { get; init; }
        public int Size { get; init; }
    }

    public class SearchAudiosRequestHandler : IRequestHandler<SearchAudiosRequest, PagedListDto<AudioViewModel>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public SearchAudiosRequestHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<PagedListDto<AudioViewModel>> Handle(SearchAudiosRequest request,
            CancellationToken cancellationToken)
        {
            var parsedTags = !string.IsNullOrWhiteSpace(request.Tags)
                ? request.Tags.Split(',')
                    .Select(t => t.Trim().ToLower())
                    .Where(t => !string.IsNullOrWhiteSpace(t))
                    .ToList()
                : new List<string>();
            
            var queryable = _unitOfWork.Audios.AsNoTracking()
                .Include(x => x.Tags)
                .Include(x => x.User)
                .Where(x => x.IsPublic);

            if (!string.IsNullOrWhiteSpace(request.Q))
                queryable = queryable.Where(a => 
                    EF.Functions.Like(a.Title.ToLower(), $"%{request.Q.ToLower()}%"));

            if (parsedTags.Count > 0)
                queryable = queryable.Where(a => a.Tags.Any(x => parsedTags.Contains(x.Name)));

            return await queryable.Select(AudioMappings.AudioToListProjection())
                .PaginateAsync(request, cancellationToken);
        }
    }
}