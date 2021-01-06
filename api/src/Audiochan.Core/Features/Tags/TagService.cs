using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Extensions;
using Audiochan.Core.Common.Models;
using Audiochan.Core.Entities;
using Audiochan.Core.Features.Tags.Models;
using Audiochan.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Audiochan.Core.Features.Tags
{
    public class TagService : ITagService
    {
        private readonly IAudiochanContext _dbContext;

        public TagService(IAudiochanContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IResult<List<PopularTagViewModel>>>  GetPopularTags(
            PaginationQuery paginationQuery
            , CancellationToken cancellationToken = default)
        {
            var queryable = (from tag in _dbContext.AudioTags.AsNoTracking()
                            group tag by tag.TagId
                            into g
                            select new PopularTagViewModel
                            {
                                Tag = g.Key,
                                Count = g.Count()
                            })
                .AsNoTracking()
                .OrderByDescending(dto => dto.Count);

            var vm = await queryable.Paginate(
                paginationQuery.Page
                , paginationQuery.Limit
                , cancellationToken);

            return Result<List<PopularTagViewModel>>.Success(vm);
        }

        public async Task<List<Tag>> CreateNewTags(IEnumerable<string?> tags, CancellationToken cancellationToken = default)
        {
            var taggifyTags = tags
                .Where(t => t != null)
                .Select(t => t!.GenerateTag())
                .ToList();

            var existingTags = await _dbContext.Tags
                .AsNoTracking()
                .Where(tag => taggifyTags.Contains(tag.Id))
                .ToListAsync(cancellationToken);
            
            var newTags = new List<Tag>();

            foreach (var tag in taggifyTags)
            {
                if (existingTags.All(t => t.Id != tag))
                    newTags.Add(new Tag{Id = tag});
            }

            await _dbContext.Tags.AddRangeAsync(newTags, cancellationToken);

            await _dbContext.SaveChangesAsync(cancellationToken);

            return existingTags.Concat(newTags).ToList();
        }
    }
}