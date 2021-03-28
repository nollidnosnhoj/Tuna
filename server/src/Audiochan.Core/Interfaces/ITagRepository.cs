using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Entities;

namespace Audiochan.Core.Interfaces
{
    public interface ITagRepository
    {
        Task<List<Tag>> GetListAsync(IEnumerable<string> tags, CancellationToken cancellationToken = default);
    }
}