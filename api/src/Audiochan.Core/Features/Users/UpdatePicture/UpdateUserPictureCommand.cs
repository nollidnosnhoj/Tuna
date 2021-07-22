using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Constants;
using Audiochan.Core.Common.Interfaces;
using Audiochan.Core.Common.Models;
using Audiochan.Core.Common.Settings;
using Audiochan.Core.Entities;
using Audiochan.Core.Services;
using MediatR;
using Microsoft.Extensions.Options;

namespace Audiochan.Core.Features.Users.UpdatePicture
{
    public record UpdateUserPictureCommand : IRequest<Result<ImageUploadResponse>>
    {
        public string UserId { get; set; } = string.Empty;
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
        private readonly IImageProcessingService _imageProcessingService;
        private readonly IStorageService _storageService;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly ICurrentUserService _currentUserService;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateUserPictureCommandHandler(IOptions<MediaStorageSettings> options,
            IImageProcessingService imageProcessingService,
            IStorageService storageService,
            IDateTimeProvider dateTimeProvider, 
            ICurrentUserService currentUserService, 
            IUnitOfWork unitOfWork)
        {
            _storageSettings = options.Value;
            _imageProcessingService = imageProcessingService;
            _storageService = storageService;
            _dateTimeProvider = dateTimeProvider;
            _currentUserService = currentUserService;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<ImageUploadResponse>> Handle(UpdateUserPictureCommand command, CancellationToken cancellationToken)
        {
            var container = string.Join('/', _storageSettings.Image.Container, "users");
            var user = await _unitOfWork.Users.LoadAsync(new object[]{command.UserId}, cancellationToken);

            if (user == null)
                return Result<ImageUploadResponse>.NotFound<User>();

            if (user.Id != _currentUserService.GetUserId())
                return Result<ImageUploadResponse>.Forbidden();
        
            var blobName = $"{user.Id}/{_dateTimeProvider.Now:yyyyMMddHHmmss}.jpg";
            
            _unitOfWork.BeginTransaction();
            try
            {
                await _imageProcessingService.UploadImage(command.Data, container, blobName, cancellationToken);

                if (!string.IsNullOrEmpty(user.PictureBlobName))
                    await _storageService.RemoveAsync(_storageSettings.Audio.Bucket, container, user.PictureBlobName,
                        cancellationToken);

                user.UpdatePicture(blobName);

                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }
            catch
            {
                _unitOfWork.RollbackTransaction();
                throw;
            }

            await _unitOfWork.CommitTransactionAsync();
            
            return Result<ImageUploadResponse>.Success(new ImageUploadResponse
            {
                Url = string.Format(MediaLinkInvariants.UserPictureUrl, blobName)
            });
        }
    }
}