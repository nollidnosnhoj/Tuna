using System.Collections.Generic;
using System.Linq;
using Ardalis.Specification;
using Audiochan.API.Features.Audios.GetAudioList;
using Audiochan.API.Mappings;
using Audiochan.Core.Entities;

namespace Audiochan.API.Features.Audios.SearchAudios
{
    public sealed class SearchAudioSpecification : Specification<Audio, AudioViewModel>
    {
        public SearchAudioSpecification(string searchTerm, IReadOnlyCollection<string> tags)
        {
            Query.AsNoTracking()
                .Include(x => x.Tags)
                .Include(x => x.User)
                .Where(x => x.IsPublic);
            
            if (!string.IsNullOrWhiteSpace(searchTerm))
                Query.Search(x => x.Title, $"%{searchTerm.Trim()}%");

            if (tags.Count > 0)
                Query.Where(a => a.Tags.Any(x => tags.Contains(x.Name)));

            Query.Select(AudioMappings.AudioToListProjection());
        }
    }
}