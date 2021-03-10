using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Extensions;
using Audiochan.Core.Common.Models.Responses;
using Audiochan.Core.Entities;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using IdentityOptions = Audiochan.Core.Common.Options.IdentityOptions;

namespace Audiochan.Core.Features.Users.UpdatePassword
{
    public record UpdatePasswordCommand : IRequest<IResult<bool>>
    {
        [JsonIgnore] public string UserId { get; init; }
        public string CurrentPassword { get; init; } = "";
        public string NewPassword { get; init; } = "";
    }

    public class UpdatePasswordCommandValidator : AbstractValidator<UpdatePasswordCommand>
    {
        public UpdatePasswordCommandValidator(IOptions<IdentityOptions> options)
        {
            RuleFor(req => req.NewPassword)
                .NotEmpty()
                .WithMessage("New Password is required.")
                .NotEqual(req => req.CurrentPassword)
                .WithMessage("New password cannot be the same as the previous.")
                .Password(options.Value, "New Password");
        }
    }

    public class UpdatePasswordCommandHandler : IRequestHandler<UpdatePasswordCommand, IResult<bool>>
    {
        private readonly UserManager<User> _userManager;

        public UpdatePasswordCommandHandler(UserManager<User> userManger)
        {
            _userManager = userManger;
        }

        public async Task<IResult<bool>> Handle(UpdatePasswordCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(request.UserId);
            if (user == null) return Result<bool>.Fail(ResultError.Unauthorized);
            // TEMPORARY UNTIL EMAIL CONFIRMATION IS SETUP
            var result = await _userManager.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);
            return result.ToResult();
        }
    }
}