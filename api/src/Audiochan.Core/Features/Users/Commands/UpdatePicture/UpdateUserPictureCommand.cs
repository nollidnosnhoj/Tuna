using System.Threading;
using System.Threading.Tasks;
using Audiochan.Common.Mediatr;
using Audiochan.Common.Exceptions;
using Audiochan.Core.Features.Upload.Dtos;
using Audiochan.Core.Persistence;
using Audiochan.Core.Services;
using MediatR;

namespace Audiochan.Core.Features.Users.Commands.UpdatePicture
{
    public class UpdateUserPictureCommand : ICommandRequest<ImageUploadResponse>
    {
        public long UserId { get; }
        public string? UploadId { get; }

        public UpdateUserPictureCommand(long userId, string? uploadId)
        {
            UserId = userId;
            UploadId = uploadId;
        }
    }

    public class UpdateUserPictureCommandHandler : IRequestHandler<UpdateUserPictureCommand, ImageUploadResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IStorageService _storageService;

        public UpdateUserPictureCommandHandler(IUnitOfWork unitOfWork, IStorageService storageService)
        {
            _unitOfWork = unitOfWork;
            _storageService = storageService;
        }

        public async Task<ImageUploadResponse> Handle(UpdateUserPictureCommand command, CancellationToken cancellationToken)
        {
            var user = await _unitOfWork.Users.FindAsync(command.UserId, cancellationToken);

            if (user is null)
            {
                throw new UnauthorizedException();
            }
        
            if (string.IsNullOrEmpty(command.UploadId))
            {
                await RemoveOriginalPicture(user.ImageId, cancellationToken);
                user.ImageId = null;
            }
            else
            {
                user.ImageId = command.UploadId;
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return ImageUploadResponse.ToUserImage(user.ImageId);
        }
        
        private async Task RemoveOriginalPicture(string? picture, CancellationToken cancellationToken = default)
        {
            if (!string.IsNullOrEmpty(picture))
            {
                var blobName = $"images/users/{picture}";
                await _storageService.RemoveAsync("audiochan", blobName, cancellationToken);
                // TODO: Add bucket to configuration
            }
        }
    }
}