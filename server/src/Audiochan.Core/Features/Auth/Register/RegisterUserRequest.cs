using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Extensions;
using Audiochan.Core.Common.Interfaces;
using Audiochan.Core.Common.Models.Interfaces;
using Audiochan.Core.Common.Settings;
using Audiochan.Core.Entities;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace Audiochan.Core.Features.Auth.Register
{
    public class RegisterUserRequest : IRequest<IResult<bool>>
    {
        public string Username { get; init; } = string.Empty;
        public string Email { get; init; } = string.Empty;
        public string Password { get; init; } = string.Empty;
    }
    
    public class RegisterUserRequestValidator : AbstractValidator<RegisterUserRequest>
    {
        public RegisterUserRequestValidator(IOptions<IdentitySettings> identitySettings)
        {
            RuleFor(req => req.Username)
                .Username(identitySettings.Value.UsernameSettings);
            RuleFor(req => req.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Email is invalid.");
            RuleFor(req => req.Password)
                .NotEmpty().WithMessage("Password is required.")
                .Password(identitySettings.Value.PasswordSettings);
        }
    }

    
    public class RegisterUserRequestHandler : IRequestHandler<RegisterUserRequest, IResult<bool>>
    {
        private readonly UserManager<User> _userManager;
        private readonly IDateTimeProvider _dateTimeProvider;

        public RegisterUserRequestHandler(UserManager<User> userManager, IDateTimeProvider dateTimeProvider)
        {
            _userManager = userManager;
            _dateTimeProvider = dateTimeProvider;
        }

        public async Task<IResult<bool>> Handle(RegisterUserRequest request, CancellationToken cancellationToken)
        {
            var user = new User(request.Username.Trim().ToLower(), request.Email, _dateTimeProvider.Now);
            var identityResult = await _userManager.CreateAsync(user, request.Password);
            return identityResult.ToResult();
        }
    }

}