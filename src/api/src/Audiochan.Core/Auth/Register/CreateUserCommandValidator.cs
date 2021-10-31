﻿using Audiochan.Core.Common;
using Audiochan.Core.Common.Extensions;
using FluentValidation;
using Microsoft.Extensions.Options;

namespace Audiochan.Core.Auth
{
    public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
    {
        public CreateUserCommandValidator(IOptions<IdentitySettings> identitySettings)
        {
            RuleFor(req => req.Username)
                .UsernameValidation(identitySettings.Value.UsernameSettings);
            RuleFor(req => req.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Email is invalid.");
            RuleFor(req => req.Password)
                .NotEmpty().WithMessage("Password is required.")
                .PasswordValidation(identitySettings.Value.PasswordSettings);
        }
    }
}