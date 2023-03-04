using Audiochan.Core.Entities;
using Audiochan.Core.Exceptions;
using Audiochan.Core.Features.Users.Models;
using Audiochan.Shared.Models;
using HotChocolate.Types.Relay;

namespace Audiochan.API.Features.Users.Errors;

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