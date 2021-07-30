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
        public string UserId { get; init; } = string.Empty;
        public string Data { get; init; } = null!;

        public static UpdateUserPictureCommand FromRequest(string userId, ImageUploadRequest request) => new()
        {
            UserId = userId,
            Data = request.Data
        };
    }

    public class UpdateUserPictureCommandHandler : IRequestHandler<UpdateUserPictureCommand, Result<ImageUploadResponse>>
    {
        private readonly MediaStorageSettings _storageSettings;
        private readonly IImageUploadService _imageUploadService;
        private readonly IStorageService _storageService;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly string _currentUserId;
        private readonly ApplicationDbContext _unitOfWork;

        public UpdateUserPictureCommandHandler(IOptions<MediaStorageSettings> options,
            IImageUploadService imageUploadService,
            IStorageService storageService,
            IDateTimeProvider dateTimeProvider, 
            ICurrentUserService currentUserService, 
            ApplicationDbContext unitOfWork)
        {
            _storageSettings = options.Value;
            _imageUploadService = imageUploadService;
            _storageService = storageService;
            _dateTimeProvider = dateTimeProvider;
            _currentUserId = currentUserService.GetUserId();
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<ImageUploadResponse>> Handle(UpdateUserPictureCommand command, CancellationToken cancellationToken)
        {
            var container = string.Join('/', _storageSettings.Image.Container, "users");
            var user = await _unitOfWork.Users.FindAsync(new object[]{command.UserId}, cancellationToken);

            if (user == null)
                return Result<ImageUploadResponse>.NotFound<User>();

            if (user.Id != _currentUserId)
                return Result<ImageUploadResponse>.Forbidden();
        
            var blobName = $"{user.Id}/{_dateTimeProvider.Now:yyyyMMddHHmmss}.jpg";
            
            await _imageUploadService.UploadImage(command.Data, container, blobName, cancellationToken);

            if (!string.IsNullOrEmpty(user.PictureBlobName))
                await _storageService.RemoveAsync(_storageSettings.Audio.Bucket, container, user.PictureBlobName,
                    cancellationToken);

            user.UpdatePicture(blobName);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<ImageUploadResponse>.Success(new ImageUploadResponse
            {
                Url = string.Format(MediaLinkInvariants.UserPictureUrl, blobName)
            });
        }
    }
}