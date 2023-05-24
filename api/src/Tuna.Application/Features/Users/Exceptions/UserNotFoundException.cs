using Tuna.Domain.Entities;
using Tuna.Domain.Exceptions;

namespace Tuna.Application.Features.Users.Exceptions;

public class UserNotFoundException : EntityNotFoundException<User, long>
{
    public UserNotFoundException(long id) : base(id)
    {
    }
}