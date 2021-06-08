using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Interfaces;
using Audiochan.Core.Services;
using MediatR;

namespace Audiochan.Core.Common.Pipelines
{
    public class CacheQueryPipelineBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : ICacheQuery
    {
        private readonly ICacheService _cacheService;

        public CacheQueryPipelineBehavior(ICacheService cacheService)
        {
            _cacheService = cacheService;
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            TResponse response;
            if (request.BypassCache) return await next();
            var cachedResponse = await _cacheService.GetAsync<TResponse>(request.CacheKey, cancellationToken);
            if (cachedResponse is not null)
            {
                response = cachedResponse;
                return response;
            }

            response = await next();
            await _cacheService.SetAsync(request.CacheKey, response, request.CacheExpiration, cancellationToken);
            return response;
        }
    }
}