using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Models;
using Audiochan.Core.Entities;
using Audiochan.Core.Services;
using MediatR;

namespace Audiochan.Core.Features.Auth.Register
{
    public class RegisterUserRequest : IRequest<Result<bool>>
    {
        public string Username { get; init; } = string.Empty;
        public string Email { get; init; } = string.Empty;
        public string Password { get; init; } = string.Empty;
    }


    public class RegisterUserRequestHandler : IRequestHandler<RegisterUserRequest, Result<bool>>
    {
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly IIdentityService _identityService;

        public RegisterUserRequestHandler(IDateTimeProvider dateTimeProvider, IIdentityService identityService)
        {
            _dateTimeProvider = dateTimeProvider;
            _identityService = identityService;
        }

        public async Task<Result<bool>> Handle(RegisterUserRequest request, CancellationToken cancellationToken)
        {
            var user = new User(request.Username.Trim().ToLower(), request.Email, _dateTimeProvider.Now);
            return await _identityService.CreateUser(user, request.Password);
        }
    }
}