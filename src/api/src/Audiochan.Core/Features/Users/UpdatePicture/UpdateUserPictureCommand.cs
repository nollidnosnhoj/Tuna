using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common;
using Audiochan.Core.Common.Interfaces;
using Audiochan.Core.Common.Models;
using Audiochan.Core.Interfaces;
using Audiochan.Core.Persistence;
using Audiochan.Domain.Entities;
using MediatR;

namespace Audiochan.Core.Features.Users.UpdatePicture
{
    public record UpdateUserPictureCommand(long UserId, string Data = "") : IImageData,
        IRequest<Result<ImageUploadResponse>>;

    public class UpdateUserPictureCommandHandler : IRequestHandler<UpdateUserPictureCommand, Result<ImageUploadResponse>>
    {
        private readonly IImageUploadService _imageUploadService;
        private readonly long _currentUserId;
        private readonly ApplicationDbContext _unitOfWork;
        private readonly IRandomIdGenerator _randomIdGenerator;

        public UpdateUserPictureCommandHandler(IImageUploadService imageUploadService,
            ICurrentUserService currentUserService, 
            ApplicationDbContext unitOfWork, 
            IRandomIdGenerator randomIdGenerator)
        {
            _imageUploadService = imageUploadService;
            _currentUserId = currentUserService.GetUserId();
            _unitOfWork = unitOfWork;
            _randomIdGenerator = randomIdGenerator;
        }

        public async Task<Result<ImageUploadResponse>> Handle(UpdateUserPictureCommand command, CancellationToken cancellationToken)
        {
            var user = await _unitOfWork.Users.FindAsync(new object[]{command.UserId}, cancellationToken);

            if (user == null)
                return Result<ImageUploadResponse>.NotFound<User>();

            if (user.Id != _currentUserId)
                return Result<ImageUploadResponse>.Forbidden();
        
            var blobName = string.Empty;
            if (string.IsNullOrEmpty(command.Data))
            {
                await RemoveOriginalPicture(user.Picture, cancellationToken);
                user.Picture = null;
            }
            else
            {
                blobName = $"{await _randomIdGenerator.GenerateAsync(size: 15)}.jpg";
                await _imageUploadService.UploadImage(command.Data, AssetContainerConstants.UserPictures, blobName, cancellationToken);
                await RemoveOriginalPicture(user.Picture, cancellationToken);
                user.Picture = blobName;
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<ImageUploadResponse>.Success(new ImageUploadResponse
            {
                Url = string.Format(MediaLinkInvariants.UserPictureUrl, blobName)
            });
        }
        
        private async Task RemoveOriginalPicture(string? picture, CancellationToken cancellationToken = default)
        {
            if (!string.IsNullOrEmpty(picture))
            {
                await _imageUploadService.RemoveImage(AssetContainerConstants.UserPictures, picture, cancellationToken);
            }
        }
    }
}