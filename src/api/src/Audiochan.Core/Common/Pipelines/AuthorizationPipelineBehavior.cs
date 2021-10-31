using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Attributes;
using Audiochan.Core.Common.Exceptions;
using Audiochan.Core.Common.Extensions;
using Audiochan.Core.Common.Interfaces.Services;
using MediatR;

namespace Audiochan.Core.Common.Pipelines
{
    public class AuthorizationPipelineBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : notnull
    {
        private readonly ICurrentUserService _currentUserService;

        public AuthorizationPipelineBehavior(ICurrentUserService currentUserService)
        {
            _currentUserService = currentUserService;
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, 
            RequestHandlerDelegate<TResponse> next)
        {
            var authorizationAttributes = request.GetType().GetCustomAttributes<AuthorizeAttribute>();

            if (!authorizationAttributes.Any())
            {
                return await next();
            }

            if (_currentUserService.User is not null)
            {
                return await next();
            }

            throw new UnauthorizedException();
        }
    }
}