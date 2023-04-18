using MediatR;

namespace Tuna.Shared.Mediatr;

public interface IQueryRequest : IRequest
{
    
}

public interface IQueryRequest<out TResponse> : IRequest<TResponse>
{
    
}