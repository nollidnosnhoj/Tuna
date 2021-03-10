using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Extensions;
using Audiochan.Core.Common.Models.Responses;
using Audiochan.Core.Entities;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Audiochan.Core.Features.Users.UpdateEmail
{
    public record UpdateEmailCommand : IRequest<IResult<bool>>
    {
        [JsonIgnore] public string UserId { get; init; }
        public string NewEmail { get; init; }
    }

    public class UpdateEmailCommandValidator : AbstractValidator<UpdateEmailCommand>
    {
        public UpdateEmailCommandValidator()
        {
            RuleFor(req => req.NewEmail)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Email is invalid.");
        }
    }

    public class UpdateEmailCommandHandler : IRequestHandler<UpdateEmailCommand, IResult<bool>>
    {
        private readonly UserManager<User> _userManager;

        public UpdateEmailCommandHandler(UserManager<User> userManger)
        {
            _userManager = userManger;
        }

        public async Task<IResult<bool>> Handle(UpdateEmailCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(request.UserId);
            if (user == null) return Result<bool>.Fail(ResultError.Unauthorized);

            // TEMPORARY UNTIL EMAIL CONFIRMATION IS SETUP
            var result = await _userManager.SetEmailAsync(user, request.NewEmail);
            if (!result.Succeeded) return result.ToResult();
            await _userManager.UpdateNormalizedEmailAsync(user);
            return Result<bool>.Success(true);
        }
    }
}