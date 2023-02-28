using Audiochan.Common.Models;
using Audiochan.Core.Features.Users.Models;
using HotChocolate.Types.Relay;

namespace Audiochan.API.Features.Users.Errors;

public record UserNotFoundError : IUserError
{
    public UserNotFoundError(long id, string? message = null)
    {
        Id = id;
        Message = message ?? $"User with id {id} was not found.";
    }
    
    [ID(nameof(UserDto))]
    public long Id { get; }
    public string Code => GetType().Name;
    public string Message { get; }
}