using System.Threading;
using System.Threading.Tasks;
using Audiochan.Application.Commons;
using Audiochan.Application.Commons.CQRS;
using Audiochan.Application.Commons.Dtos.Responses;
using Audiochan.Application.Commons.Services;
using Audiochan.Application.Persistence;
using KopaCore.Result;
using KopaCore.Result.Errors;
using MediatR;

namespace Audiochan.Application.Features.Users.Commands.RemovePicture
{
    public record RemoveUserPictureCommand(long UserId) : ICommandRequest<Result>;
    
    public class RemoveUserPictureCommandHandler : IRequestHandler<RemoveUserPictureCommand, Result>
    {
        private readonly IImageService _imageService;
        private readonly IUnitOfWork _unitOfWork;

        public RemoveUserPictureCommandHandler(IImageService imageService, IUnitOfWork unitOfWork)
        {
            _imageService = imageService;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(RemoveUserPictureCommand command, CancellationToken cancellationToken)
        {
            var user = await _unitOfWork.Users.FindAsync(command.UserId, cancellationToken);

            if (user!.Id != command.UserId)
                return new ForbiddenErrorResult();

            if (string.IsNullOrEmpty(user.Picture)) return new SuccessResult();
            
            await _imageService.RemoveImage(AssetContainerConstants.USER_PICTURES, user.Picture, cancellationToken);

            user.Picture = null;

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new SuccessResult();
        }
    }
}