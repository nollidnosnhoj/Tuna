using System;
using System.Linq;
using Ardalis.Specification;
using Audiochan.API.Features.Shared.Helpers;
using Audiochan.API.Mappings;
using Audiochan.Core.Entities;

namespace Audiochan.API.Features.Audios.GetAudioList
{
    public sealed class GetAudioListSpecification : Specification<Audio, AudioViewModel>
    {
        public GetAudioListSpecification(string? cursor, int size = 30, string? tag = null)
        {
            Query.AsNoTracking()
                .Include(a => a.User)
                .Where(a => a.IsPublic);

            if (!string.IsNullOrWhiteSpace(cursor))
            {
                var (since, id) = CursorHelpers.DecodeCursor(cursor);
                if (Guid.TryParse(id, out var audioId) && since.HasValue)
                {
                    Query.Where(a => a.Created < since.GetValueOrDefault() 
                                    || a.Created == since.GetValueOrDefault() && a.Id.CompareTo(audioId) < 0);
                }
            }

            if (!string.IsNullOrWhiteSpace(tag))
            {
                Query.Where(a => a.Tags.Any(t => t.Name == tag));
            }

            Query.Select(AudioMappings.AudioToListProjection())
                .OrderByDescending(a => a.Created)
                .ThenByDescending(a => a.Id)
                .Take(size);
        }
    }
}