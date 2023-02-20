using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Common.Mediatr;
using Audiochan.Common.Exceptions;
using Audiochan.Core.Features.Users.Dtos;
using Audiochan.Core.Persistence;
using MediatR;

namespace Audiochan.Core.Features.Users.Commands.UpdateProfile
{
    public class UpdateProfileCommand : AuthCommandRequest<UserDto>
    {
        public string? DisplayName { get; }

        public UpdateProfileCommand(string? displayName, ClaimsPrincipal user) : base(user)
        {
            DisplayName = displayName;
        }
    }

    public class UpdateProfileCommandHandler : IRequestHandler<UpdateProfileCommand, UserDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdateProfileCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<UserDto> Handle(UpdateProfileCommand command, CancellationToken cancellationToken)
        {
            var userId = command.GetUserId();
            
            var user = await _unitOfWork.Users.FindAsync(userId, cancellationToken);
            
            if (user is null)
            {
                throw new UnauthorizedException();
            }

            if (user.Id != userId)
            {
                throw new UnauthorizedException();
            }
            
            // TODO: Update user stuff

            return new UserDto
            {
                Id = userId,
                Picture = user.ImageId,
                UserName = user.UserName
            };
        }
    }
}