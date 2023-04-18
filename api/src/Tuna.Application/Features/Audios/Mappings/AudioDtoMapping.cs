using System.Linq;
using Tuna.Application.Entities;
using Tuna.Application.Features.Audios.Models;

namespace Tuna.Application.Features.Audios.Mappings;

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
            ImageId = audio.ImageId,
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
            ImageId = audio.ImageId,
            Size = audio.FileSize,
            Title = audio.Title,
            UserId = audio.UserId
        };
    }
}