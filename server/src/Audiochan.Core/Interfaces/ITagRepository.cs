using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Entities;

namespace Audiochan.Core.Interfaces
{
    public interface ITagRepository
    {
        Task<List<Tag>> CreateTags(IEnumerable<string> tags, CancellationToken cancellationToken = default);
    }
}