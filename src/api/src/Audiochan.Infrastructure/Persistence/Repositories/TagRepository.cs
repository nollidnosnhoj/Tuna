using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Interfaces.Persistence;
using Audiochan.Domain.Entities;
using Audiochan.Infrastructure.Persistence.Repositories.Abstractions;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace Audiochan.Infrastructure.Persistence.Repositories
{
    public class TagRepository : EfRepository<Tag>, ITagRepository
    {
        public TagRepository(ApplicationDbContext dbContext, IMapper mapper) : base(dbContext, mapper)
        {
        }

        public async Task<List<Tag>> GetAppropriateTags(IEnumerable<string> tags, CancellationToken cancellationToken = default)
        {
            var tagEntities = await DbSet
                .Where(tag => tags.Contains(tag.Name))
                .ToListAsync(cancellationToken);
            
            foreach (var tag in tags.Where(tag => tagEntities.All(t => t.Name != tag)))
            {
                tagEntities.Add(new Tag {Name = tag});
            }

            return tagEntities;
        }
    }
}