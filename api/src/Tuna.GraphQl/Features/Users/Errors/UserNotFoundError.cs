using HotChocolate.Types.Relay;
using Tuna.Application.Exceptions;
using Tuna.Application.Features.Users.Models;
using Tuna.Domain;
using Tuna.Domain.Entities;
using Tuna.Domain.Exceptions;
using Tuna.Shared.Models;

namespace Tuna.GraphQl.Features.Users.Errors;

public record UserNotFoundError : IUserError
{
    public UserNotFoundError(EntityNotFoundException<User, long> ex)
    {
        Id = ex.Id;
        Code = GetType().Name;
        Message = ex.Message;
    }

    [ID(nameof(UserDto))] public long Id { get; }

    public string Code { get; }
    public string Message { get; }
}