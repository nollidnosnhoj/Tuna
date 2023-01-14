using MediatR;

namespace Audiochan.Common.CQRS;

public interface IQueryRequest : IRequest
{
    
}

public interface IQueryRequest<out TResponse> : IRequest<TResponse>
{
    
}