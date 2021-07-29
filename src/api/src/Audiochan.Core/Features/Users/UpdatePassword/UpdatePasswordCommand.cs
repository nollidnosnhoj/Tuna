using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Extensions;
using Audiochan.Core.Common.Models;
using Audiochan.Core.Common.Settings;
using Audiochan.Core.Entities;
using Audiochan.Core.Interfaces;
using Audiochan.Core.Persistence;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace Audiochan.Core.Features.Users.UpdatePassword
{
    public record UpdatePasswordCommand : IRequest<Result<bool>>
    {
        public string UserId { get; init; } = string.Empty;
        public string CurrentPassword { get; init; } = "";
        public string NewPassword { get; init; } = "";

        public static UpdatePasswordCommand FromRequest(string userId, UpdatePasswordRequest request) => new()
        {
            UserId = userId,
            CurrentPassword = request.CurrentPassword,
            NewPassword = request.NewPassword
        };
    }
    
    public class UpdatePasswordCommandValidator : AbstractValidator<UpdatePasswordCommand>
    {
        public UpdatePasswordCommandValidator(IOptions<IdentitySettings> options)
        {
            RuleFor(req => req.NewPassword)
                .NotEmpty()
                .WithMessage("New Password is required.")
                .NotEqual(req => req.CurrentPassword)
                .WithMessage("New password cannot be the same as the previous.")
                .Password(options.Value.PasswordSettings, "New Password");
        }
    }

    public class UpdatePasswordCommandHandler : IRequestHandler<UpdatePasswordCommand, Result<bool>>
    {
        private readonly string _currentUserId;
        private readonly ApplicationDbContext _unitOfWork;
        private readonly UserManager<User> _userManager;

        public UpdatePasswordCommandHandler(ICurrentUserService currentUserService, ApplicationDbContext unitOfWork, 
            UserManager<User> userManager)
        {
            _currentUserId = currentUserService.GetUserId();
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

        public async Task<Result<bool>> Handle(UpdatePasswordCommand command, CancellationToken cancellationToken)
        {
            var user = await _unitOfWork.Users.FindAsync(new object[]{command.UserId}, cancellationToken);
            if (user == null) return Result<bool>.Unauthorized();
            if (user.Id != _currentUserId)
                return Result<bool>.Forbidden();

            return (await _userManager.ChangePasswordAsync(user, command.CurrentPassword, command.NewPassword)).ToResult();
        }
    }
}