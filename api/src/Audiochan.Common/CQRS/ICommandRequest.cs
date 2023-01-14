using MediatR;

namespace Audiochan.Common.CQRS;

public interface ICommandRequest : IRequest
{
    
}

public interface ICommandRequest<out TResponse> : IRequest<TResponse>
{
    
}