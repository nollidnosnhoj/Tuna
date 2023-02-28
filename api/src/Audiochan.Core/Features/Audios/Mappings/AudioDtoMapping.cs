using System.Linq;
using Audiochan.Core.Features.Audios.Models;
using Audiochan.Domain.Entities;

namespace Audiochan.Core.Features.Audios.Mappings;

public static class Mappings
{
    public static IQueryable<AudioViewModel> ProjectToDto(this IQueryable<Audio> queryable)
    {
        return queryable.Select(audio => new AudioViewModel
        {
            Id = audio.Id,
            Description = audio.Description ?? "",
            ObjectKey = audio.ObjectKey,
            Created = audio.CreatedAt,
            Duration = audio.Duration,
            Picture = audio.ImageId,
            Size = audio.Size,
            Title = audio.Title
        });
    }

    public static AudioViewModel MapToDto(this Audio audio)
    {
        return new AudioViewModel
        {
            Id = audio.Id,
            Description = audio.Description ?? "",
            ObjectKey = audio.ObjectKey,
            Created = audio.CreatedAt,
            Duration = audio.Duration,
            Picture = audio.ImageId,
            Size = audio.Size,
            Title = audio.Title
        };
    }
}