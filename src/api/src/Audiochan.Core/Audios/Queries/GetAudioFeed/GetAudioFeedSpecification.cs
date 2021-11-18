using System.Linq;
using Ardalis.Specification;
using Audiochan.Domain.Entities;

namespace Audiochan.Core.Audios.Queries
{
    public sealed class GetAudioFeedSpecification : Specification<Audio>
    {
        public GetAudioFeedSpecification(long[] artistIds)
        {
            Query.AsNoTracking();
            Query.Where(a => artistIds.Contains(a.ArtistId));
            Query.OrderByDescending(a => a.Created);
        }
    }
}