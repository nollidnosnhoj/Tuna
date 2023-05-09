using HotChocolate.Types.Relay;
using Tuna.Application.Exceptions;
using Tuna.Application.Features.Audios.Models;
using Tuna.Domain;
using Tuna.Domain.Entities;
using Tuna.Domain.Exceptions;
using Tuna.Shared.Models;

namespace Tuna.GraphQl.Features.Audios.Errors;

public class AudioNotFoundError : IUserError
{
    public AudioNotFoundError(EntityNotFoundException<Audio, long> ex)
    {
        Id = ex.Id;
        Code = GetType().Name;
        Message = ex.Message;
    }

    [ID(nameof(AudioDto))] public long Id { get; }

    public string Code { get; }
    public string Message { get; }
}