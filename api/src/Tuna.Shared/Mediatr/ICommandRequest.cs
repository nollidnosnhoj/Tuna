using System.Security.Claims;
using MediatR;
using Tuna.Shared.Extensions;

namespace Tuna.Shared.Mediatr;

public interface ICommandRequest : IRequest
{
}

public interface ICommandRequest<out TResponse> : IRequest<TResponse>
{
}

public abstract class AuthCommandRequest<TResponse> : ICommandRequest<TResponse>
{
    protected AuthCommandRequest(ClaimsPrincipal user)
    {
        User = user;
    }

    public ClaimsPrincipal User { get; }

    public long GetUserId()
    {
        return User.GetUserId();
    }

    public string GetUserName()
    {
        return User.GetUserName();
    }
}