using System.Security.Claims;
using Audiochan.Shared.Extensions;
using MediatR;

namespace Audiochan.Shared.Mediatr;

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

    public long GetUserId()
    {
        return User.GetUserId();
    }
    
    public string GetUserName()
    {
        return User.GetUserName();
    }
    
    public ClaimsPrincipal User { get; }
}