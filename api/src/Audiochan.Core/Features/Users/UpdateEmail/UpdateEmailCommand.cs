using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Extensions;
using Audiochan.Core.Common.Interfaces;
using Audiochan.Core.Common.Models;
using Audiochan.Core.Entities;
using Audiochan.Core.Interfaces;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Audiochan.Core.Features.Users.UpdateEmail
{
    public record UpdateEmailCommand : IRequest<Result<bool>>
    {
        public string UserId { get; init; } = null!;
        public string NewEmail { get; init; } = null!;

        public static UpdateEmailCommand FromRequest(string userId, UpdateEmailRequest request) => new()
        {
            UserId = userId,
            NewEmail = request.NewEmail
        };
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

    public class UpdateEmailCommandHandler : IRequestHandler<UpdateEmailCommand, Result<bool>>
    {
        private readonly string _currentUserId;
        private readonly ApplicationDbContext _unitOfWork;
        private readonly UserManager<User> _userManager;

        public UpdateEmailCommandHandler(ICurrentUserService currentUserService, ApplicationDbContext unitOfWork, 
            UserManager<User> userManager)
        {
            _currentUserId = currentUserService.GetUserId();
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

        public async Task<Result<bool>> Handle(UpdateEmailCommand command, CancellationToken cancellationToken)
        {
            var user = await _unitOfWork.Users.FindAsync(new object[]{command.UserId}, cancellationToken);
            if (user == null) return Result<bool>.Unauthorized();
            if (user.Id != _currentUserId)
                return Result<bool>.Forbidden();

            var result = await _userManager.SetEmailAsync(user, command.NewEmail);
            
            if (result.Succeeded)
            {
                await _userManager.UpdateNormalizedEmailAsync(user);
            }

            return result.ToResult();
        }
    }
}