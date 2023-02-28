using Audiochan.API.Features.Audios.Errors;
using Audiochan.Domain.Entities;
using Audiochan.Domain.Exceptions;
using HotChocolate.Types;

namespace Audiochan.API.Features.Audios;

public class AudioErrorFactory
    : IPayloadErrorFactory<EntityNotFoundException<Audio, long>, AudioNotFoundError>
        , IPayloadErrorFactory<AudioNotUploadedException, AudioNotUploadedError>
{
    public AudioNotFoundError CreateErrorFrom(EntityNotFoundException<Audio, long> exception)
    {
        return new AudioNotFoundError(exception.Id);
    }

    public AudioNotUploadedError CreateErrorFrom(AudioNotUploadedException exception)
    {
        return new AudioNotUploadedError();
    }
}