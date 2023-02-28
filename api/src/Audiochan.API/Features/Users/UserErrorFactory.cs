using Audiochan.API.Features.Users.Errors;
using Audiochan.Core.Features.Auth.Exceptions;
using Audiochan.Domain.Entities;
using Audiochan.Domain.Exceptions;
using HotChocolate.Types;

namespace Audiochan.API.Features.Users;

public class UserErrorFactory
    : IPayloadErrorFactory<EntityNotFoundException<User, long>, UserNotFoundError>
    , IPayloadErrorFactory<IdentityException, IdentityErrors>
    , IPayloadErrorFactory<CannotFollowYourselfException, CannotFollowYourselfError>
{
    public UserNotFoundError CreateErrorFrom(EntityNotFoundException<User, long> exception)
    {
        return new UserNotFoundError(exception.Id);
    }

    public IdentityErrors CreateErrorFrom(IdentityException exception)
    {
        return new IdentityErrors(exception.Errors);
    }

    public CannotFollowYourselfError CreateErrorFrom(CannotFollowYourselfException exception)
    {
        return new CannotFollowYourselfError();
    }
}