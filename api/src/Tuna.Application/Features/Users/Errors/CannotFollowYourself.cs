using Tuna.Shared.Models;

namespace Tuna.Application.Features.Users.Errors;

public struct CannotFollowYourself : IUserError
{
    public string Code => GetType().Name;
    public string Message => "Cannot follow yourself.";
}