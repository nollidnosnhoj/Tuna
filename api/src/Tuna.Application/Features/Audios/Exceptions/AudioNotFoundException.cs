using Tuna.Domain.Entities;
using Tuna.Domain.Exceptions;

namespace Tuna.Application.Features.Audios.Exceptions;

public class AudioNotFoundException : EntityNotFoundException<Audio, long>
{
    public AudioNotFoundException(long id) : base(id)
    {
    }
}