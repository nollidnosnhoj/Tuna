using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common;
using Audiochan.Core.Common.Interfaces.Persistence;
using Audiochan.Core.Common.Interfaces.Services;
using Audiochan.Core.Common.Models;
using Audiochan.Domain.Entities;
using MediatR;

namespace Audiochan.Core.Users.RemovePicture
{
    public record RemoveUserPictureCommand(long UserId) : IRequest<Result>;
    
    public class RemoveUserPictureCommandHandler : IRequestHandler<RemoveUserPictureCommand, Result>
    {
        private readonly IImageUploadService _imageUploadService;
        private readonly IUnitOfWork _unitOfWork;

        public RemoveUserPictureCommandHandler(IImageUploadService imageUploadService, IUnitOfWork unitOfWork)
        {
            _imageUploadService = imageUploadService;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(RemoveUserPictureCommand command, CancellationToken cancellationToken)
        {
            var user = await _unitOfWork.Users.FindAsync(command.UserId, cancellationToken);

            if (user == null)
                return Result<ImageUploadResponse>.NotFound<User>();

            if (user.Id != command.UserId)
                return Result<ImageUploadResponse>.Forbidden();

            if (string.IsNullOrEmpty(user.Picture)) return Result.Success();
            
            await _imageUploadService.RemoveImage(AssetContainerConstants.UserPictures, user.Picture, cancellationToken);

            user.Picture = null;

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}