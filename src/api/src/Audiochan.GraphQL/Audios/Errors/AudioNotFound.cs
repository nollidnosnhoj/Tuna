using Audiochan.Application.Commons.Exceptions;
using Audiochan.Domain.Entities;
using Audiochan.GraphQL.Common.Errors;

namespace Audiochan.GraphQL.Audios.Errors;

public class AudioNotFound : GraphQlError
{
    public long AudioId { get; }
    
    public AudioNotFound(NotFoundException<Audio, long> exception) 
        : base($"Resource is not found. Type: {exception.Type.Name}. Id: {exception.ResourceId}")
    {
        AudioId = exception.ResourceId;
    }
}