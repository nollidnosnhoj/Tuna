using System.Collections.Generic;
using Ardalis.Specification;
using Audiochan.API.Features.Audios.GetAudioList;
using Audiochan.API.Mappings;
using Audiochan.Core.Entities;

namespace Audiochan.API.Features.Audios.GetAudioFeed
{
    public sealed class GetAudioFeedSpecification : Specification<Audio, AudioViewModel>
    {
        public GetAudioFeedSpecification(List<string> followedIds)
        {
            Query.AsNoTracking()
                .Include(a => a.User)
                .Where(a => a.IsPublic)
                .Where(a => followedIds.Contains(a.UserId))
                .OrderByDescending(a => a.Created);

            Query.Select(AudioMappings.AudioToListProjection());
        }
    }
}