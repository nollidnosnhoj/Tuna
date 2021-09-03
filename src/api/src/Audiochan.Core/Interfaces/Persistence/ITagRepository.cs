using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Domain.Entities;

namespace Audiochan.Core.Interfaces.Persistence
{
    public interface ITagRepository : IEntityRepository<Tag>
    {
        Task<List<Tag>> GetAppropriateTags(IEnumerable<string> tags, CancellationToken cancellationToken = default);
    }
}