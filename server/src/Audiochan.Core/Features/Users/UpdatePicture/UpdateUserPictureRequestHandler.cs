using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Helpers;
using Audiochan.Core.Common.Models.Responses;
using Audiochan.Core.Common.Options;
using Audiochan.Core.Entities;
using Audiochan.Core.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace Audiochan.Core.Features.Users.UpdatePicture
{
    public class UpdateUserPictureRequestHandler : IRequestHandler<UpdateUserPictureRequest, IResult<string>>
    {
        private readonly AudiochanOptions.StorageOptions _pictureStorageOptions;
        private readonly UserManager<User> _userManager;
        private readonly IImageService _imageService;
        private readonly IStorageService _storageService;
        private readonly IDateTimeProvider _dateTimeProvider;

        public UpdateUserPictureRequestHandler(IOptions<AudiochanOptions> options,
            UserManager<User> userManager,
            IImageService imageService,
            IStorageService storageService,
            IDateTimeProvider dateTimeProvider)
        {
            _pictureStorageOptions = options.Value.ImageStorageOptions;
            _userManager = userManager;
            _imageService = imageService;
            _storageService = storageService;
            _dateTimeProvider = dateTimeProvider;
        }

        public async Task<IResult<string>> Handle(UpdateUserPictureRequest request, CancellationToken cancellationToken)
        {
            var container = Path.Combine(_pictureStorageOptions.Container, "users");
            var blobName = BlobHelpers.GetPictureBlobName(_dateTimeProvider.Now);
            try
            {
                var user = await _userManager.FindByIdAsync(request.UserId + "");
                if (user == null) return Result<string>.Fail(ResultError.Unauthorized);

                if (!string.IsNullOrEmpty(user.Picture))
                {
                    await _storageService.RemoveAsync(user.Picture, cancellationToken);
                    user.UpdatePicture(string.Empty);
                }

                var image = await _imageService.UploadImage(request.ImageData, container, blobName, cancellationToken);
                user.UpdatePicture(image.Path);
                await _userManager.UpdateAsync(user);
                return Result<string>.Success(image.Url);
            }
            catch (Exception)
            {
                await _storageService.RemoveAsync(container, blobName, cancellationToken);
                throw;
            }
        }
    }
}