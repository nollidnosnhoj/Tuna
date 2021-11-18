using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Attributes;
using Audiochan.Core.Common.Exceptions;
using Audiochan.Core.Common.Extensions;
using Audiochan.Core.Common.Interfaces.Persistence;
using Audiochan.Core.Common.Interfaces.Services;
using MediatR;

namespace Audiochan.Core.Common.Pipelines
{
    public class AuthorizationPipelineBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : notnull
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IUnitOfWork _unitOfWork;

        public AuthorizationPipelineBehavior(ICurrentUserService currentUserService, IUnitOfWork unitOfWork)
        {
            _currentUserService = currentUserService;
            _unitOfWork = unitOfWork;
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, 
            RequestHandlerDelegate<TResponse> next)
        {
            var authorizationAttributes = request.GetType()
                .GetCustomAttributes<AuthorizeAttribute>().ToList();

            // If a command requires authorization
            if (authorizationAttributes.Any())
            {
                if (_currentUserService.User is null)
                    throw new UnauthorizedException();
                
                _currentUserService.User.TryGetUserId(out var userId);

                var artistAuthorizationAttributes = authorizationAttributes
                    .Where(a => a.RequiresArtist);

                // If a command requires artist authorization
                if (artistAuthorizationAttributes.Any())
                {
                    var isArtist = await _unitOfWork.Artists
                        .ExistsAsync(u => u.Id == userId, cancellationToken);
                    
                    if (!isArtist)
                    {
                        throw new ForbiddenAccessException();
                    }
                }
            }

            return await next();
        }
    }
}