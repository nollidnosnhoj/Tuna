using Ardalis.Specification;
using Audiochan.Core.Common.Mappings;
using Audiochan.Core.Entities;
using Audiochan.Core.Features.Audios.GetAudioList;

namespace Audiochan.Core.Features.Users.GetUserAudios
{
    public sealed class GetUserAudiosSpecification : Specification<Audio, AudioViewModel>
    {
        public GetUserAudiosSpecification(string? username, string currentUserId = "")
        {
            Query.AsNoTracking()
                .Include(a => a.User)
                .Where(a => username != null && username == a.User.UserName.ToLower());

            if (!string.IsNullOrEmpty(currentUserId))
            {
                Query.Where(a => a.IsPublic || a.UserId == currentUserId);
            }
            else
            {
                Query.Where(a => a.IsPublic);
            }

            Query.Select(AudioMappings.AudioToListProjection());
        }
    }
}