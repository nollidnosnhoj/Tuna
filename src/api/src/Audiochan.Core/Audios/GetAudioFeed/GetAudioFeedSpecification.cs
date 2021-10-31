using System.Linq;
using Ardalis.Specification;
using Audiochan.Domain.Entities;

namespace Audiochan.Core.Audios
{
    public sealed class GetAudioFeedSpecification : Specification<Audio>
    {
        public GetAudioFeedSpecification(long[] userIds)
        {
            Query.AsNoTracking();
            Query.Where(a => userIds.Contains(a.UserId));
            Query.OrderByDescending(a => a.Created);
        }
    }
}