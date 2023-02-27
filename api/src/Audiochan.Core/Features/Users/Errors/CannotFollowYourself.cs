using Audiochan.Common.Models;

namespace Audiochan.Core.Features.Users.Errors;

public struct CannotFollowYourself : IUserError
{
    public string Code => GetType().Name;
    public string Message => "Cannot follow yourself.";
}