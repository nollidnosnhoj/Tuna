using MediatR;

namespace Audiochan.Common.Mediatr;

public interface IQueryRequest : IRequest
{
    
}

public interface IQueryRequest<out TResponse> : IRequest<TResponse>
{
    
}