using System;

namespace Audiochan.Core.Features.Audios.Exceptions;

public class AudioNotFoundException : Exception
{
    public long Id { get; }
    public AudioNotFoundException(long id) : base("Audio was not found.")
    {
        Id = id;
    }
}