using System.Threading;
using System.Threading.Tasks;
using Audiochan.Application.Commons.CQRS;
using Audiochan.Application.Commons.Exceptions;
using Audiochan.Application.Persistence;
using Audiochan.Application.Services;
using MediatR;

namespace Audiochan.Application.Features.Users.Commands.RemovePicture
{
    public record RemoveUserPictureCommand(long UserId) : ICommandRequest;
    
    public class RemoveUserPictureCommandHandler : IRequestHandler<RemoveUserPictureCommand>
    {
        private readonly IImageService _imageService;
        private readonly IUnitOfWork _unitOfWork;

        public RemoveUserPictureCommandHandler(IImageService imageService, IUnitOfWork unitOfWork)
        {
            _imageService = imageService;
            _unitOfWork = unitOfWork;
        }

        public async Task<Unit> Handle(RemoveUserPictureCommand command, CancellationToken cancellationToken)
        {
            var user = await _unitOfWork.Users.FindAsync(command.UserId, cancellationToken);

            if (user!.Id != command.UserId)
                throw new ForbiddenException();

            if (string.IsNullOrEmpty(user.Picture)) return Unit.Value;

            await _imageService.RemoveImage(AssetContainerConstants.USER_PICTURES, user.Picture, cancellationToken);

            user.Picture = null;

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}