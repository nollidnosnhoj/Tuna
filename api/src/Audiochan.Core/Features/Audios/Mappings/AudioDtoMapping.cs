using System.Linq;
using Audiochan.Core.Entities;
using Audiochan.Core.Features.Audios.Models;

namespace Audiochan.Core.Features.Audios.Mappings;

public static class Mappings
{
    public static IQueryable<AudioDto> ProjectToDto(this IQueryable<Audio> queryable)
    {
        return queryable.Select(audio => new AudioDto
        {
            Id = audio.Id,
            Description = audio.Description ?? "",
            ObjectKey = audio.FileId,
            CreatedAt = audio.CreatedAt,
            UpdatedAt = audio.UpdatedAt,
            Duration = audio.Duration,
            Picture = audio.ImageId,
            Size = audio.FileSize,
            Title = audio.Title,
            UserId = audio.UserId
        });
    }

    public static AudioDto MapToDto(this Audio audio)
    {
        return new AudioDto
        {
            Id = audio.Id,
            Description = audio.Description ?? "",
            ObjectKey = audio.FileId,
            CreatedAt = audio.CreatedAt,
            UpdatedAt = audio.UpdatedAt,
            Duration = audio.Duration,
            Picture = audio.ImageId,
            Size = audio.FileSize,
            Title = audio.Title,
            UserId = audio.UserId
        };
    }
}