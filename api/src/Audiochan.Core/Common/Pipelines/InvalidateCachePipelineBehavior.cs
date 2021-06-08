using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Interfaces;
using Audiochan.Core.Services;
using MediatR;

namespace Audiochan.Core.Common.Pipelines
{
    public class InvalidateCachePipelineBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IInvalidateCache
    {
        private readonly ICacheService _cacheService;

        public InvalidateCachePipelineBehavior(ICacheService cacheService)
        {
            _cacheService = cacheService;
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            var response = await next();
            
            if (request.InvalidCacheKeys.Length > 0)
            {
                foreach (var key in request.InvalidCacheKeys)
                {
                    await _cacheService.RemoveAsync(key, cancellationToken);
                }
            }

            return response;
        }
    }
}