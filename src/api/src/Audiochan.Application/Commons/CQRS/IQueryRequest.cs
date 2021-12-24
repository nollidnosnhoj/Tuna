using MediatR;

namespace Audiochan.Application.Commons.CQRS;

public interface IQueryRequest : IRequest
{
    
}

public interface IQueryRequest<out TResponse> : IRequest<TResponse>
{
    
}