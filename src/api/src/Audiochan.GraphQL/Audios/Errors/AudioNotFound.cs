using Audiochan.Application.Features.Audios.Exceptions;
using Audiochan.GraphQL.Common.Errors;

namespace Audiochan.GraphQL.Audios.Errors;

public class AudioNotFound : GraphQlError
{
    public long? AudioId { get; }
    public string? Slug { get; }
    
    public AudioNotFound(AudioNotFoundException exception) 
        : base(exception.Message)
    {
        AudioId = exception.AudioId;
        Slug = exception.Slug;
    }
}