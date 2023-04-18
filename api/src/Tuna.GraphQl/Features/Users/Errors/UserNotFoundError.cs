using Tuna.Application.Entities;
using Tuna.Application.Exceptions;
using Tuna.Application.Features.Users.Models;
using Tuna.Shared.Models;
using HotChocolate.Types.Relay;

namespace Tuna.GraphQl.Features.Users.Errors;

public record UserNotFoundError : IUserError
{
    public UserNotFoundError(EntityNotFoundException<User, long> ex)
    {
        Id = ex.Id;
        Code = GetType().Name;
        Message = ex.Message;
    }
    
    [ID(nameof(UserDto))]
    public long Id { get; }
    public string Code { get; }
    public string Message { get; }
}