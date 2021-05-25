using Ardalis.Specification;
using Audiochan.Core.Common.Extensions.MappingExtensions;
using Audiochan.Core.Entities;
using Audiochan.Core.Features.Audios;

namespace Audiochan.Core.Features.Users.GetUserAudios
{
    public sealed class GetUserAudiosSpecification : Specification<Audio, AudioViewModel>
    {
        public GetUserAudiosSpecification(string? username, string currentUserId = "", int size = 15)
        {
            Query.Select(AudioMappingExtensions.AudioToListProjection())
                .AsNoTracking()
                .Include(a => a.User)
                .Where(a => a.IsPublic)
                .Where(a => username != null && username == a.User.UserName.ToLower() )
                .Take(size);

            if (!string.IsNullOrEmpty(currentUserId))
            {
                Query.Where(a => a.IsPublic || a.UserId == currentUserId);
            }
            else
            {
                Query.Where(a => a.IsPublic);
            }
        }
    }
}