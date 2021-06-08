using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Models;
using Audiochan.Core.Entities;
using Audiochan.Core.Services;
using MediatR;

namespace Audiochan.Core.Features.Auth.CreateUser
{
    public class CreateUserCommand : IRequest<Result<bool>>
    {
        public string Username { get; init; } = string.Empty;
        public string Email { get; init; } = string.Empty;
        public string Password { get; init; } = string.Empty;
    }


    public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, Result<bool>>
    {
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly IIdentityService _identityService;

        public CreateUserCommandHandler(IDateTimeProvider dateTimeProvider, IIdentityService identityService)
        {
            _dateTimeProvider = dateTimeProvider;
            _identityService = identityService;
        }

        public async Task<Result<bool>> Handle(CreateUserCommand command, CancellationToken cancellationToken)
        {
            var user = new User(command.Username.Trim().ToLower(), command.Email, _dateTimeProvider.Now);
            return await _identityService.CreateUser(user, command.Password);
        }
    }
}