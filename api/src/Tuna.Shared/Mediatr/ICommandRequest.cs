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
    public ClaimsPrincipal? User { get; private set; }
    public long UserId => User.GetUserId();
    public bool IsAuthenticated => User is not null && User.Identity?.IsAuthenticated == true;

    public void SetAuthenticated(ClaimsPrincipal user)
    {
        User = user;
    }
}