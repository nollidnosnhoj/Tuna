using System.Threading;
using System.Threading.Tasks;
using Audiochan.Application.Commons;
using Audiochan.Application.Commons.CQRS;
using Audiochan.Application.Commons.Services;
using Audiochan.Application.Persistence;
using Audiochan.Application.Commons.Extensions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Audiochan.Application.Features.Users.Commands.UpdateUsername
{
    public record UpdateUsernameCommand : ICommandRequest<Result>
    {
        public long UserId { get; init; }
        public string NewUsername { get; init; } = null!;

        public static UpdateUsernameCommand FromRequest(long userId, UpdateUsernameRequest request) => new()
        {
            UserId = userId,
            NewUsername = request.NewUsername
        };
    }

    public class UpdateUsernameCommandHandler : IRequestHandler<UpdateUsernameCommand, Result>
    {
        private readonly long _currentUserId;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ApplicationDbContext _dbContext;

        public UpdateUsernameCommandHandler(ICurrentUserService currentUserService,
            IUnitOfWork unitOfWork, ApplicationDbContext dbContext)
        {
            currentUserService.User.TryGetUserId(out _currentUserId);
            _unitOfWork = unitOfWork;
            _dbContext = dbContext;
        }

        public async Task<Result> Handle(UpdateUsernameCommand command, CancellationToken cancellationToken)
        {
            var user = await _unitOfWork.Users.FindAsync(command.UserId, cancellationToken);
            if (user!.Id != _currentUserId) return Result.Forbidden();
            
            // check if username already exists
            var usernameExists =
                await _dbContext.Users.AnyAsync(u => u.UserName == command.NewUsername, cancellationToken);
            if (usernameExists)
                return Result.BadRequest("Username already exists");

            // update username
            user.UserName = command.NewUsername;

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }
    }
}