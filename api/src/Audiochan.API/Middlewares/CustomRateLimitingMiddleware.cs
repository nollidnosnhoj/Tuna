using System.Text.Json;
using System.Threading.Tasks;
using AspNetCoreRateLimit;
using Audiochan.API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;

namespace Audiochan.API.Middlewares
{
    public class CustomRateLimitingMiddleware : IpRateLimitMiddleware
    {
        private readonly IpRateLimitOptions _rateLimitOptions;

        public CustomRateLimitingMiddleware(RequestDelegate next,
            IOptions<IpRateLimitOptions> options,
            IRateLimitCounterStore counterStore,
            IIpPolicyStore policyStore,
            IRateLimitConfiguration config,
            ILogger<IpRateLimitMiddleware> logger)
            : base(next, options, counterStore, policyStore, config, logger)
        {
            _rateLimitOptions = options.Value;
        }

        public override Task ReturnQuotaExceededResponse(HttpContext httpContext, RateLimitRule rule, string retryAfter)
        {
            var message = string.Format(_rateLimitOptions.QuotaExceededResponse.Content, retryAfter);
            if (!_rateLimitOptions.DisableRateLimitHeaders)
                httpContext.Response.Headers["Retry-After"] = (StringValues) retryAfter;
            var code = _rateLimitOptions.QuotaExceededResponse?.StatusCode ?? _rateLimitOptions.HttpStatusCode;
            var contentType = _rateLimitOptions.QuotaExceededResponse?.ContentType ?? "text/plain";
            var response = new ErrorApiResponse(code, message, null);
            httpContext.Response.StatusCode = code;
            httpContext.Response.ContentType = contentType;
            return httpContext.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    }
}