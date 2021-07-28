using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Extensions;
using Audiochan.Core.Common.Interfaces;
using Audiochan.Core.Common.Models;
using Audiochan.Core.Common.Settings;
using Audiochan.Core.Entities;
using Audiochan.Core.Interfaces;
using Audiochan.Core.Persistence;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace Audiochan.Core.Features.Users.UpdateUsername
{
    public record UpdateUsernameCommand : IRequest<Result<bool>>
    {
        public string UserId { get; init; } = null!;
        public string NewUsername { get; init; } = null!;

        public static UpdateUsernameCommand FromRequest(string userId, UpdateUsernameRequest request) => new()
        {
            UserId = userId,
            NewUsername = request.NewUsername
        };
    }
    
    public class UpdateUsernameCommandValidator : AbstractValidator<UpdateUsernameCommand>
    {
        public UpdateUsernameCommandValidator(IOptions<IdentitySettings> options)
        {
            RuleFor(req => req.NewUsername).Username(options.Value.UsernameSettings);
        }
    }

    public class UpdateUsernameCommandHandler : IRequestHandler<UpdateUsernameCommand, Result<bool>>
    {
        private readonly string _currentUserId;
        private readonly ApplicationDbContext _unitOfWork;
        private readonly UserManager<User> _userManager;

        public UpdateUsernameCommandHandler(ICurrentUserService currentUserService,
            ApplicationDbContext unitOfWork, UserManager<User> userManager)
        {
            _currentUserId = currentUserService.GetUserId();
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

        public async Task<Result<bool>> Handle(UpdateUsernameCommand command, CancellationToken cancellationToken)
        {
            var user = await _unitOfWork.Users.FindAsync(new object[]{command.UserId}, cancellationToken);
            if (user == null) return Result<bool>.Unauthorized();
            if (user.Id != _currentUserId)
                return Result<bool>.Forbidden();

            // update username
            var result = await _userManager.SetUserNameAsync(user, command.NewUsername);
            
            if (result.Succeeded)
            {
                await _userManager.UpdateNormalizedUserNameAsync(user);
                user.DisplayName = user.UserName;
                await _userManager.UpdateAsync(user);
            }

            return result.ToResult();
        }
    }
}