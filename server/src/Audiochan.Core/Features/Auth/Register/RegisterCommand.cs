using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Extensions;
using Audiochan.Core.Common.Models.Responses;
using Audiochan.Core.Entities;
using Audiochan.Core.Interfaces;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using IdentityOptions = Audiochan.Core.Common.Options.IdentityOptions;

namespace Audiochan.Core.Features.Auth.Register
{
    public class RegisterCommand : IRequest<IResult<bool>>
    {
        public string Username { get; init; } = string.Empty;
        public string Email { get; init; } = string.Empty;
        public string Password { get; init; } = string.Empty;
    }

    public class RegisterCommandValidator : AbstractValidator<RegisterCommand>
    {
        public RegisterCommandValidator(IOptions<IdentityOptions> options)
        {
            RuleFor(req => req.Username).Username(options.Value);
            RuleFor(req => req.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Email is invalid.");
            RuleFor(req => req.Password)
                .NotEmpty().WithMessage("Password is required.")
                .Password(options.Value);
        }
    }

    public class RegisterCommandHandler : IRequestHandler<RegisterCommand, IResult<bool>>
    {
        private readonly UserManager<User> _userManager;
        private readonly IDateTimeService _dateTimeService;

        public RegisterCommandHandler(UserManager<User> userManager, IDateTimeService dateTimeService)
        {
            _userManager = userManager;
            _dateTimeService = dateTimeService;
        }

        public async Task<IResult<bool>> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            var user = new User(request.Username.Trim().ToLower(), request.Email, _dateTimeService.Now);
            var identityResult = await _userManager.CreateAsync(user, request.Password);
            return identityResult.ToResult();
        }
    }
}