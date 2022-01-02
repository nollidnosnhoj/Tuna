using Audiochan.Application.Commons.Exceptions;

namespace Audiochan.Application.Features.Audios.Exceptions;

public class AudioNotFoundException : NotFoundException
{
    public long? AudioId { get; }
    public string? Slug { get; }

    public AudioNotFoundException(long audioId) : base($"Unable to find audio with id: {audioId}")
    {
        AudioId = audioId;
    }

    public AudioNotFoundException(string slug) : base($"Unable to find audio with slug: {slug}")
    {
        Slug = slug;
    }
}