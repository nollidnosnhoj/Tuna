using Audiochan.Common.Models;
using Audiochan.Core.Features.Users.Models;
using Audiochan.Domain.Entities;
using Audiochan.Domain.Exceptions;
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