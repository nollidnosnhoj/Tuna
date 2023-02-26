using System.Linq;
using Audiochan.Core.Entities;
using Audiochan.Core.Features.Audios.Models;
using Audiochan.Core.Features.Users.Models;

namespace Audiochan.Core.Features.Audios.Mappings;

public static class Mappings
{
    public static IQueryable<AudioViewModel> Project(this IQueryable<Audio> queryable)
    {
        return queryable.Select(x => new AudioViewModel
        {
            Id = x.Id,
            Description = x.Description ?? "",
            ObjectKey = x.ObjectKey,
            Created = x.CreatedAt,
            Duration = x.Duration,
            Picture = x.ImageId,
            Size = x.Size,
            Title = x.Title,
            User = new UserViewModel
            {
                Id = x.UserId,
                Picture = x.User.ImageId,
                UserName = x.User.UserName
            }
        });
    }
}