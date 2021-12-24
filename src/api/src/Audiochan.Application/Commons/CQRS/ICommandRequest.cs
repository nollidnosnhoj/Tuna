using MediatR;

namespace Audiochan.Application.Commons.CQRS;

public interface ICommandRequest : IRequest
{
    
}

public interface ICommandRequest<out TResponse> : IRequest<TResponse>
{
    
}