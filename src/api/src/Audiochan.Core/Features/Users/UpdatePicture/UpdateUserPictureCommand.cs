using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Constants;
using Audiochan.Core.Common.Interfaces;
using Audiochan.Core.Common.Models;
using Audiochan.Core.Common.Settings;
using Audiochan.Core.Entities;
using Audiochan.Core.Interfaces;
using Audiochan.Core.Persistence;
using MediatR;
using Microsoft.Extensions.Options;

namespace Audiochan.Core.Features.Users.UpdatePicture
{
    public record UpdateUserPictureCommand : IImageData, IRequest<Result<ImageUploadResponse>>
    {
        public long UserId { get; init; }
        public string Data { get; init; } = null!;

        public static UpdateUserPictureCommand FromRequest(long userId, ImageUploadRequest request) => new()
        {
            UserId = userId,
            Data = request.Data
        };
    }

    public class UpdateUserPictureCommandHandler : IRequestHandler<UpdateUserPictureCommand, Result<ImageUploadResponse>>
    {
        private readonly MediaStorageSettings.StorageSettings _storageSettings;
        private readonly IImageUploadService _imageUploadService;
        private readonly long _currentUserId;
        private readonly ApplicationDbContext _unitOfWork;
        private readonly INanoidGenerator _nanoidGenerator;

        public UpdateUserPictureCommandHandler(IOptions<MediaStorageSettings> mediaStorageOptions,
            IImageUploadService imageUploadService,
            ICurrentUserService currentUserService, 
            ApplicationDbContext unitOfWork, INanoidGenerator nanoidGenerator)
        {
            var mediaStorageSettings = mediaStorageOptions.Value;
            _storageSettings = mediaStorageSettings.Image;
            _imageUploadService = imageUploadService;
            _currentUserId = currentUserService.GetUserId();
            _unitOfWork = unitOfWork;
            _nanoidGenerator = nanoidGenerator;
        }

        public async Task<Result<ImageUploadResponse>> Handle(UpdateUserPictureCommand command, CancellationToken cancellationToken)
        {
            var container = string.Join('/', _storageSettings.Container, "users");
            var user = await _unitOfWork.Users.FindAsync(new object[]{command.UserId}, cancellationToken);

            if (user == null)
                return Result<ImageUploadResponse>.NotFound<User>();

            if (user.Id != _currentUserId)
                return Result<ImageUploadResponse>.Forbidden();
        
            var blobName = $"{await _nanoidGenerator.GenerateAsync(size: 15)}.jpg";
            
            await _imageUploadService.UploadImage(command.Data, container, blobName, cancellationToken);

            if (!string.IsNullOrEmpty(user.PictureBlobName))
                await _imageUploadService.RemoveImage(container, user.PictureBlobName, cancellationToken);

            user.PictureBlobName = blobName;

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<ImageUploadResponse>.Success(new ImageUploadResponse
            {
                Url = string.Format(MediaLinkInvariants.UserPictureUrl, blobName)
            });
        }
    }
}