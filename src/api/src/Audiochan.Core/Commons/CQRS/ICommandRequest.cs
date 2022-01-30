using MediatR;

namespace Audiochan.Core.Commons.CQRS;

public interface ICommandRequest : IRequest
{
    
}

public interface ICommandRequest<out TResponse> : IRequest<TResponse>
{
    
}