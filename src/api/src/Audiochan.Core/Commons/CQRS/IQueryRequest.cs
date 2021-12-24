using MediatR;

namespace Audiochan.Core.Commons.CQRS;

public interface IQueryRequest : IRequest
{
    
}

public interface IQueryRequest<out TResponse> : IRequest<TResponse>
{
    
}