using MediatR;

namespace Audiochan.Common.Mediatr;

public interface ICommandRequest : IRequest
{
    
}

public interface ICommandRequest<out TResponse> : IRequest<TResponse>
{
    
}