using MediatR;

namespace Audiochan.Shared.Mediatr;

public interface IQueryRequest : IRequest
{
    
}

public interface IQueryRequest<out TResponse> : IRequest<TResponse>
{
    
}